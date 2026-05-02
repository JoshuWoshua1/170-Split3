using UnityEngine;
using System.Collections.Generic;
using TMPro;

public enum ResourceType
{
    Water,
    Glucose,
    AminoAcids,
    Deoxyribose
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>(); // dictionary for storing resource types and their quantities

    [SerializeField] private TextMeshProUGUI debugText; // debug text for displaying resource info. remove when UI scripts are made.

    private void Awake() // singleton behavior
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of ResourceManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugText != null)
            debugText.text = GetResourceSummary(); // debug display. remove when UI scripts are made.
    }

    void InitializeDictionary()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0; // initialize all resource types to 0 in the dictionary.
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (resources.ContainsKey(type))
        {
            resources[type] += amount; // add specified amount to the resource type in the dictionary.
        } else Debug.LogWarning("Resource type " + type + " not found in dictionary.");
    }

    public string GetResourceSummary()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder("Resources Debug:\n");
        foreach (var kv in resources)
            sb.AppendLine($"{kv.Key}: {kv.Value}");
        return sb.ToString();
    }
}
