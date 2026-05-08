using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ResourceUIScript : MonoBehaviour
{

    [SerializeField] private ElementUI UIPrefab;
    [SerializeField] private Transform UITransform;


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
        
    }

    void InitializeUI()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            ElementUI newElement = Instantiate(UIPrefab, UITransform);
            newElement.Load(type, ResourceManager.Instance.GetResourceAmount(type));
        }
    }
}
