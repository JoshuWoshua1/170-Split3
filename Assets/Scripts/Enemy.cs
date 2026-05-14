using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float chaseMoveSpeed = 3f;
    public float runAwayMoveSpeed = 2.5f;
    public float roamMoveSpeed = 2f;
    private Transform playerTransform;
    public float trackingRange = 5f;
    public float runAwayBuffer = 2f; // extra distance to maintain when running away to avoid getting stuck
    public float sizeRequirement = 1f; // player size required to consume this enemy 
    [SerializeField] private ResourceType[] resourceType; // type(s) of resource this enemy provides
    [SerializeField] private int[] resourceAmount = {1}; // amount(s) of resource this enemy provides
    private bool canDoDamage = true; // flag to control whether the enemy can currently damage the player
    private bool isTracking = false; // flag to indicate if the enemy is currently tracking the player
    private bool isRunningAway = false; // flag to track if currently in runaway state
    [SerializeField] private float roamNoiseScale = 0.35f; // controls how quickly roam direction changes
    [SerializeField] private float roamSmoothing = 4f; // smooths roaming movement
    private Vector2 roamNoiseOffset; // unique noise seed
    private Vector3 currentRoamDirection;
    private bool spawnTracking = true; // flag to prevent roaming and player scanning until spawn tracking is complete
    
    ResourceManager resourceManager;
    Collider enemyCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resourceManager = ResourceManager.Instance; // get instance of resource manager.
        enemyCollider = GetComponent<Collider>();
        roamNoiseOffset = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        currentRoamDirection = Random.insideUnitSphere;
        currentRoamDirection.y = 0f;
        if (currentRoamDirection.sqrMagnitude < 0.0001f)
        {
            currentRoamDirection = Vector3.forward;
        }
        else
        {
            currentRoamDirection.Normalize();
        }

        if (PlayerController.Instance != null)
            playerTransform = PlayerController.Instance.transform;
        else
            Debug.LogError("PlayerController.Instance is null!", this);
        

        StartCoroutine(SpawnTrack());
        //Debug.LogError("test");
        gameObject.transform.localScale = sizeRequirement * 0.9f * Vector3.one; // scale enemy size based on size requirement
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTracking) return;
        if (!isTracking)
        {
            Roam();
        }
        ScanForPlayer();
        CheckSize();
    }
    // CHANGE ALL OF THIS SO THAT ITS MORE LIKE A STATE MACHINE SO CHASING ISNT WEIRD

    private IEnumerator SpawnTrack()
    {
        float spawnTrackDuration = 2f; // duration of spawn tracking in seconds
        float elapsed = 0f;

        while (elapsed < spawnTrackDuration)
        {
            if (playerTransform != null)
            {
                MoveTowardsPlayer();
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        spawnTracking = false; // end spawn tracking and allow normal behavior
    }

    private void Roam()
    {
        float t = Time.time * roamNoiseScale;
        float noiseX = Mathf.PerlinNoise(roamNoiseOffset.x + t, roamNoiseOffset.y) - 0.5f;
        float noiseZ = Mathf.PerlinNoise(roamNoiseOffset.x, roamNoiseOffset.y + t) - 0.5f;

        Vector3 targetDirection = new Vector3(noiseX, 0f, noiseZ);
        if (targetDirection.sqrMagnitude > 0.0001f)
        {
            targetDirection.Normalize();
            currentRoamDirection = Vector3.Slerp(currentRoamDirection, targetDirection, roamSmoothing * Time.deltaTime).normalized;
        }

        transform.position += currentRoamDirection * roamMoveSpeed * Time.deltaTime;
    }

    private void ScanForPlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = HorizontalDistanceToPlayer();
        //Debug.Log($"Enemy scanning. distance={distanceToPlayer:F2}, range={trackingRange:F2}", this);

        if (isRunningAway)
        {
            // Keep running until safely past the buffer
            if (distanceToPlayer > trackingRange + runAwayBuffer)
            {
                isRunningAway = false;
                isTracking = false;
            }
            else
            {
                RunAwayFromPlayer();
            }
        }
        else if (distanceToPlayer <= trackingRange)
        {
            if (sizeRequirement > PlayerController.Instance.size)
            {
                MoveTowardsPlayer();
                isTracking = true;
            }
            else
            {
                isRunningAway = true;
                isTracking = true;
                RunAwayFromPlayer();
            }
        }
        else if (isTracking)
        {
            isTracking = false;
        }
    }

    private void MoveTowardsPlayer()
    {
        isTracking = true;
        //Debug.Log("Enemy is moving towards the player!");
        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.y = 0f; // ignore height differences for simple ground tracking
        Vector3 direction = toPlayer.normalized;
        transform.position += direction * chaseMoveSpeed * Time.deltaTime;
    }

    private void RunAwayFromPlayer()
    {
        isTracking = true;
        Vector3 awayFromPlayer = transform.position - playerTransform.position;
        awayFromPlayer.y = 0f; // ignore height differences for simple ground tracking
        Vector3 direction = awayFromPlayer.normalized;
        transform.position += direction * runAwayMoveSpeed * Time.deltaTime;
    }

    private float HorizontalDistanceToPlayer()
    {
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = playerTransform.position;
        enemyPos.y = 0f;
        playerPos.y = 0f;
        return Vector3.Distance(enemyPos, playerPos);
    }

    private void CheckSize()
    {
        if (PlayerController.Instance.size >= sizeRequirement)
        {
            canDoDamage = false; // disable damage to player if they are large enough to consume the enemy.
        } else canDoDamage = true; // enable damage if player is too small to consume enemy.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canDoDamage)
            {
                Debug.Log("Enemy damaged the player!");
            } else eatEnemy();
        }
    }

    private void eatEnemy()
    {
        foreach (ResourceType type in resourceType)
        {
            int index = System.Array.IndexOf(resourceType, type);
            if (index >= 0 && index < resourceAmount.Length)
            {
                resourceManager.AddResource(type, resourceAmount[index]); // add resource to resource manager.
                GameManager.Instance.AddResourcesGained(resourceAmount[index]); // add to resource gain count in game manager.
            }
        }
        //resourceManager.AddResource(resourceType, resourceAmount); // add resource to resource manager.
        GameManager.Instance.AddEaten(1);
        Destroy(gameObject); // destroy object after consumption.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
}
