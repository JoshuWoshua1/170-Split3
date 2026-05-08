using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ResourceUIScript : MonoBehaviour
{

    [SerializeField] private ElementUI UIPrefab;
    [SerializeField] private Transform UITransform;

    private Dictionary<ResourceType, ElementUI> UIElements = new Dictionary<ResourceType, ElementUI>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager doesn't exist. Cannot load resources.");
        }

        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            checkResource(type, ResourceManager.Instance.GetResourceAmount(type));
        }
    }

    void InitializeUI()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            ElementUI newElement = Instantiate(UIPrefab, UITransform);
            newElement.Load(type, ResourceManager.Instance.GetResourceAmount(type));
            newElement.gameObject.SetActive(false);

            UIElements.Add(type, newElement);
        }
    }

    void checkResource(ResourceType type, int amnt)
    {
        if (UIElements.TryGetValue(type, out ElementUI UI))
        {
            if (UI.getAmount() > 0 && !UI.gameObject.activeSelf)
            {
                UI.gameObject.SetActive(true);
            }
            UI.setAmount(amnt, "set");
        }
    }
}
