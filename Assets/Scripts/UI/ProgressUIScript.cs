using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ProgressUIScript : MonoBehaviour
{
    [Tooltip("This needs to be the same sizeUpData object used on the player for accurate progress tracking!")]
    [SerializeField] private SizeUp_Data sizeUpData; //this needs to be the same sizeUpData object used on the player

    [SerializeField] private ProgressUI UIPrefab;
    [SerializeField] private Transform UITransform;
    [SerializeField] private Transform UITransformParent;

    private Dictionary<ResourceType, ProgressUI> UIElements = new Dictionary<ResourceType, ProgressUI>();

    private SizeTier tier = null;

    private int currentTier = 0;

    private bool meetsThresholds = false;

    private bool updating = false;

    private bool showingPanel = true;

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
        if (!updating)
        {
            foreach (ResourceRequirement requirement in tier.resourceThresholds)
            {
                CheckResourceThresholds(requirement.resourceType, requirement.amount, ResourceManager.Instance.GetResourceAmount(requirement.resourceType));
            }
        }
    }

    void InitializeUI()
    {
        //starts at the first tier in sizeUpData
        tier = sizeUpData.tiers.FirstOrDefault();

        foreach (ResourceRequirement requirement in tier.resourceThresholds)
        {
            ProgressUI newProgress = Instantiate(UIPrefab, UITransform);
            newProgress.Load(requirement.resourceType, requirement.amount, 0);

            UIElements.Add(requirement.resourceType, newProgress);
        }
    }

    public void updateProgress()
    {
        updating = true;

        UIElements.Clear();

        currentTier++;

        tier = sizeUpData.tiers[currentTier];

        foreach (Transform child in UITransform)
        {
            Destroy(child.gameObject);
        }

        foreach (ResourceRequirement requirement in tier.resourceThresholds)
        {
            ProgressUI newProgress = Instantiate(UIPrefab, UITransform);
            newProgress.Load(requirement.resourceType, 0, requirement.amount);

            UIElements.Add(requirement.resourceType, newProgress);
        }

        updating = false;
    }

    private void CheckResourceThresholds(ResourceType type, int amnt, int curr)
    {
        foreach (ResourceRequirement requirement in tier.resourceThresholds)
        {
            meetsThresholds = true;
            if (ResourceManager.Instance.GetResourceAmount(requirement.resourceType) < requirement.amount)
            {
                meetsThresholds = false;
                if (UIElements.TryGetValue(type, out ProgressUI UI))
                {
                    UI.setAmount(curr, amnt, "set");
                }
                break;
            }
        }
        if (meetsThresholds)
        {
            if (UIElements.TryGetValue(type, out ProgressUI UI))
            {
                UI.setAmount(curr, amnt, "set");
            }
            Debug.Log("updatimg progress UI");
            updateProgress();
        }
    }

    void hideProg()
    {
        showingPanel = false;
        UITransformParent.transform.DOMoveY(-150, 1f).SetEase(Ease.OutQuart);
    }

    void showProg()
    {
        showingPanel=true;
        UITransformParent.transform.DOMoveY(66, 1f).SetEase(Ease.OutQuad);
    }

    public void ToggleProgressPanel()
    {
        if (showingPanel)
        {
            hideProg();
        }
        else
        {
            showProg();
        }
    }
}

