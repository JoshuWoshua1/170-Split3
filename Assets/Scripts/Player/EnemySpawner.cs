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
    public float despawnRadius = 20f;
    private float playerSizeScale = 1f; // scale spawn/despawn radius based on player size
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSizeScale = PlayerController.Instance.size; // initialize player size scale
    }

    // Update is called once per frame
    void Update()
    {
        spawnRadius *= playerSizeScale; // scale spawn radius based on player size
        despawnRadius *= playerSizeScale; // scale despawn radius based on player size
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, despawnRadius);
    }
}
