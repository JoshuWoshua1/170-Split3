using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float sizeRequirement = 1f; // player size required to consume this object 
    [SerializeField] private ResourceType[] resourceType; // type(s) of resource this object provides
    [SerializeField] private int[] resourceAmount = {1}; // amount(s) of resource this object provides

    ResourceManager resourceManager;
    Collider objectCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resourceManager = ResourceManager.Instance; // get instance of resource manager.
        objectCollider = GetComponent<Collider>();
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
            objectCollider.isTrigger = true; // enable collider to allow player to consume object.
        } else objectCollider.isTrigger = false; // disable collider to prevent player from consuming object.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
    }

}
