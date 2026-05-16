using UnityEngine;

public class ROG : MonoBehaviour
{
    [System.Serializable]
    private struct ResourceEntry
    {
        public GameObject prefab;
        public int count;
    }

    [SerializeField] private ResourceEntry[] resources;

    [SerializeField] private Vector2 spawnArea = new Vector2(100f, 100f);
    [SerializeField] private float spawnHeight = 0.2f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float spawnZoneRadius = 2f; // Radius around the player where resources will not spawn

    private System.Collections.Generic.List<Vector3> spawnedPositions = new System.Collections.Generic.List<Vector3>();

    void Start()
    {
        SpawnResources();
    }

    private void SpawnResources()
    {
        int total = 0;
        foreach (ResourceEntry entry in resources)
        {
            if (entry.prefab != null)
            {
                SpawnResourceType(entry.prefab, entry.count);
                total += entry.count;
            }
        }

        Debug.Log($"ROG: Spawned {total} total resources");
    }

    private void SpawnResourceType(GameObject prefab, int count)
    {
        if (prefab == null)
        {
            Debug.LogWarning("ROG: Prefab not assigned!");
            return;
        }

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = count * 20;

        while (spawned < count && attempts < maxAttempts)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnArea.x * 0.5f, spawnArea.x * 0.5f),
                spawnHeight,
                Random.Range(-spawnArea.y * 0.5f, spawnArea.y * 0.5f)
            );

            if (IsPositionValid(randomPosition))
            {
                Instantiate(prefab, randomPosition, Quaternion.identity, transform);
                spawnedPositions.Add(randomPosition);
                spawned++;
            }

            attempts++;
        }

        if (spawned < count)
        {
            Debug.LogWarning($"ROG: Only spawned {spawned}/{count} of {prefab.name} after {maxAttempts} attempts");
        }
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 existingPos in spawnedPositions)
        {
            if (Vector3.Distance(position, existingPos) < minDistance)
            {
                return false;
            }
        }
        if (Vector3.Distance(position, Vector3.zero) < spawnZoneRadius)
        {
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(0, spawnHeight, 0), new Vector3(spawnArea.x, 0.1f, spawnArea.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, spawnZoneRadius);
    }
}
