using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float chaseMoveSpeed = 2f;
    public float runAwayMoveSpeed = 2.5f;
    private Transform playerTransform;
    public float trackingRange = 3f;
    public float sizeRequirement = 1f; // player size required to consume this enemy 
    [SerializeField] private ResourceType[] resourceType; // type(s) of resource this enemy provides
    [SerializeField] private int[] resourceAmount = {1}; // amount(s) of resource this enemy provides
    private bool canDoDamage = true; // flag to control whether the enemy can currently damage the player
    private bool isTracking = false; // flag to indicate if the enemy is currently tracking the player
    
    ResourceManager resourceManager;
    Collider enemyCollider;

    [SerializeField] private int updateFrameTimer = 5; // how many frames between update checks
    private int frameCounter = 0; // counter to track frames for update checks
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resourceManager = ResourceManager.Instance; // get instance of resource manager.
        enemyCollider = GetComponent<Collider>();
        if (PlayerController.Instance != null)
            playerTransform = PlayerController.Instance.transform;
        else
            Debug.LogError("PlayerController.Instance is null!", this);
        
        //Debug.LogError("test");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTracking)
        {
            Roam();
        }
        ScanForPlayer();
        CheckSize();
    }
    // CHANGE ALL OF THIS SO THAT ITS MORE LIKE A STATE MACHINE SO CHASING ISNT WEIRD

    private void Roam()
    {
        // Simple roaming behavior: move in a random direction for a short duration
        Vector3 randomDirection = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, 0f, Mathf.PerlinNoise(0f, Time.time) - 0.5f).normalized;
        transform.position += randomDirection * chaseMoveSpeed * Time.deltaTime;
    }

    private void ScanForPlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = HorizontalDistanceToPlayer();
        //Debug.Log($"Enemy scanning. distance={distanceToPlayer:F2}, range={trackingRange:F2}", this);

        if (distanceToPlayer <= trackingRange && sizeRequirement > PlayerController.Instance.size)
        {
            MoveTowardsPlayer();
            //Debug.Log("Enemy is tracking the player!");
        }
        else if (distanceToPlayer <= trackingRange && sizeRequirement <= PlayerController.Instance.size)
        {
            RunAwayFromPlayer();
            //Debug.Log("Enemy is running away from the player!");
        }
        else if (isTracking)
        {
            isTracking = false;
            //Debug.Log("Enemy lost track of the player.");
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
            }
        }
        //resourceManager.AddResource(resourceType, resourceAmount); // add resource to resource manager.
        Destroy(gameObject); // destroy object after consumption.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
}
