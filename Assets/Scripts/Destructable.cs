using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float sizeRequirement = 1f; // player size required to consume this object 
    [SerializeField] private ResourceType resourceType; // type of resource this object provides
    [SerializeField] private int resourceAmount = 1; // amount of resource this object provides

    ResourceManager resourceManager;
    Collider collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resourceManager = ResourceManager.Instance; // get instance of resource manager.
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSize();
    }

    private void CheckSize()
    {
        if (PlayerController.Instance.size >= sizeRequirement)
        {
            collider.isTrigger = true; // enable collider to allow player to consume object.
        } else collider.isTrigger = false; // disable collider to prevent player from consuming object.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            resourceManager.AddResource(resourceType, resourceAmount); // add resource to resource manager.
            Destroy(gameObject); // destroy object after consumption.
        }
    }

}
