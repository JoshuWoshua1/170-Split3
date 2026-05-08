using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem; 

public class ResourceUIScript : MonoBehaviour
{

    [SerializeField] private ElementUI UIPrefab;
    [SerializeField] private Transform UITransform;
    [SerializeField] private Transform UITransformParent;

    private Dictionary<ResourceType, ElementUI> UIElements = new Dictionary<ResourceType, ElementUI>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DOTween.Init(autoKillMode, useSafeMode, logBehaviour); 

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

        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame) //change this later
        {
            Debug.Log("showing panel");
            showPanel();
        }
    }

    void InitializeUI()
    {
        UITransformParent.gameObject.SetActive(false);

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
            if (UI.getAmount() > 0 && !UI.gameObject.activeSelf) //shows the UI element when the value is greater than 0 and it was not active; essentially adds the element to the UI when it's "discovered" for the first time
            {
                UI.gameObject.SetActive(true);
            }
            UI.setAmount(amnt, "set");
        }
    }

    void showPanel() {
        UITransformParent.gameObject.SetActive(true);

        UITransformParent.DOMoveX(-10, 2).From();
    }
}
