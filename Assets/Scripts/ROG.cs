using UnityEngine;

public class ROG : MonoBehaviour
{
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject glucosePrefab;
    [SerializeField] private GameObject aminoAcidPrefab;
    [SerializeField] private GameObject deoxyribosePrefab;

    [SerializeField] private int waterCount = 25;
    [SerializeField] private int glucoseCount = 15;
    [SerializeField] private int aminoAcidCount = 10;
    [SerializeField] private int deoxyriboseCount = 5;

    [SerializeField] private Vector2 spawnArea = new Vector2(100f, 100f);
    [SerializeField] private float spawnHeight = 0.2f;
    [SerializeField] private float minDistance = 1.5f;

    private System.Collections.Generic.List<Vector3> spawnedPositions = new System.Collections.Generic.List<Vector3>();

    void Start()
    {
        SpawnResources();
    }

    private void SpawnResources()
    {
        SpawnResourceType(waterPrefab, waterCount);
        SpawnResourceType(glucosePrefab, glucoseCount);
        SpawnResourceType(aminoAcidPrefab, aminoAcidCount);
        SpawnResourceType(deoxyribosePrefab, deoxyriboseCount);

        Debug.Log($"ROG: Spawned {waterCount + glucoseCount + aminoAcidCount + deoxyriboseCount} total resources");
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
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(0, spawnHeight, 0), new Vector3(spawnArea.x, 0.1f, spawnArea.y));
    }
}
