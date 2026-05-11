using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of EnemySpawner detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    public GameObject enemyPrefab;
    public float spawnRadius = 15f;
    public float despawnRadius = 30f;
    private float playerSizeScale; // scale spawn/despawn radius based on player size
    [SerializeField] private float spawnInterval = 2f; // time between spawns
    [SerializeField] private int maxEnemies = 10; // maximum number of enemies in the scene at once.
    private float spawnTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSizeScale = PlayerController.Instance.size; // initialize player size scale
    }

    // Update is called once per frame
    void Update()
    {
        checkPlayerScale();
    }

    private void checkPlayerScale()
    {
        if (PlayerController.Instance != null)
        {
            float currentPlayerSize = PlayerController.Instance.size;
            if (Mathf.Abs(currentPlayerSize - playerSizeScale) > 0.01f) // check for size change
            {
                playerSizeScale = currentPlayerSize;
                spawnRadius *= playerSizeScale; // scale spawn radius based on player size
                despawnRadius *= playerSizeScale; // scale despawn radius based on player size
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, despawnRadius);
    }
}
