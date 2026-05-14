using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    public float spawnRadius = 15f; // base spawn radius before player-size scaling
    public float despawnRadius = 30f; // base despawn radius before player-size scaling
    [SerializeField] private float scaleMultiplier = 0.5f; // scale up = playersize*scaleMultiplier for spawn/despawn radius
    private float playerSizeScale; // scale spawn/despawn radius based on player size
    private float scaledSpawnRadius;
    private float scaledDespawnRadius;
    [SerializeField] private float spawnInterval = 2f; // time between spawns
    [SerializeField] private int maxEnemies = 10; // maximum number of enemies in the scene at once.
    [SerializeField] private int maxLargeEnemies = 3; // cap for enemies larger than current player size
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
        if (enemyPrefabs.Count == 0) return;

        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        int currentEnemyCount = existingEnemies.Length;
        if (currentEnemyCount >= maxEnemies) return; // dont spawn anything past max enemy count

        float currentPlayerSize = PlayerController.Instance.size;
        float minSpawnSize = Mathf.Max(1f, currentPlayerSize - 1f); // clamp min enemy sze to 1 below player
        float maxSpawnSize = currentPlayerSize + 1f; // clamp max enemy size to 1 above player size

        int largerEnemyCount = 0;

        foreach (Enemy enemy in existingEnemies)
        {
            if (enemy == null) continue;

            if (enemy.sizeRequirement > currentPlayerSize)
            {
                largerEnemyCount++;
                // Debug.Log("Large enemies:" + largerEnemyCount);
            }
        }

        List<GameObject> candidatePrefabs = new List<GameObject>();
        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab == null) continue;

            Enemy prefabEnemy = prefab.GetComponent<Enemy>();
            if (prefabEnemy == null) continue;

            float prefabSize = prefabEnemy.sizeRequirement;
            if (prefabSize < minSpawnSize || prefabSize > maxSpawnSize)
            {
                continue;
            }

            if (prefabSize > currentPlayerSize && largerEnemyCount >= maxLargeEnemies)
            {
                continue;
            }

            candidatePrefabs.Add(prefab);
        }

        if (candidatePrefabs.Count == 0) return;

        GameObject selectedPrefab = candidatePrefabs[Random.Range(0, candidatePrefabs.Count)];
        //Debug.Log("Spawning enemy of size: " + selectedPrefab.GetComponent<Enemy>().sizeRequirement);

        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0f; // keep spawns on the same y axis
        randomDirection.Normalize();
        Vector3 spawnPos = playerPos + randomDirection * scaledSpawnRadius;

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
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
