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
    public float spawnRadius = 15f; // base spawn radius before player-size scaling
    public float despawnRadius = 30f; // base despawn radius before player-size scaling
    [SerializeField] private float scaleMultiplier = 0.5f; // scale up = playersize*scaleMultiplier for spawn/despawn radius
    private float playerSizeScale; // scale spawn/despawn radius based on player size
    private float scaledSpawnRadius;
    private float scaledDespawnRadius;
    [SerializeField] private float spawnInterval = 2f; // time between spawns
    [SerializeField] private int maxEnemies = 10; // maximum number of enemies in the scene at once.
    private float spawnTimer;
    private float despawnTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSizeScale = PlayerController.Instance.size;
        RecalculateScale();
    }

    // Update is called once per frame
    void Update()
    {
        checkPlayerScale();
        DespawnFarEnemies();

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
        despawnTimer += Time.deltaTime;
        if (despawnTimer >= 1f) // check for despawning every second
        {
            despawnTimer = 0f;
            DespawnFarEnemies();
        }
    }

    private void TrySpawnEnemy()
    {
        if (PlayerController.Instance == null) return;

        int currentEnemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if (currentEnemyCount >= maxEnemies) return; // dont spawn anything past max enemy count

        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0f; // keep spawns on the same y axis
        randomDirection.Normalize();
        Vector3 spawnPos = playerPos + randomDirection * scaledSpawnRadius;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private void DespawnFarEnemies()
    {
        if (PlayerController.Instance == null) return;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        float despawnDistanceSqr = scaledDespawnRadius * scaledDespawnRadius;
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            Vector3 toEnemy = enemy.transform.position - playerPos;
            toEnemy.y = 0f; // ignore vertical offset for top-down distance checks

            if (toEnemy.sqrMagnitude > despawnDistanceSqr)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    private void checkPlayerScale()
    {
        if (PlayerController.Instance != null)
        {
            float currentPlayerSize = PlayerController.Instance.size;
            if (Mathf.Abs(currentPlayerSize - playerSizeScale) > 0.01f) // check for size change
            {
                playerSizeScale = currentPlayerSize;
                RecalculateScale();
            }
        }
    }

    private void RecalculateScale()
    {
        scaledSpawnRadius = spawnRadius + (playerSizeScale - 1f) * scaleMultiplier;
        scaledDespawnRadius = despawnRadius + (playerSizeScale - 1f) * scaleMultiplier;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = PlayerController.Instance != null ? PlayerController.Instance.transform.position : transform.position;
        float previewSpawnRadius = Application.isPlaying ? scaledSpawnRadius : spawnRadius;
        float previewDespawnRadius = Application.isPlaying ? scaledDespawnRadius : despawnRadius;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, previewSpawnRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, previewDespawnRadius);
    }
}
