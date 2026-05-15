using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform recipeListParent;

    [Header("Colors")]
    [SerializeField] private Color craftableAccent = new Color(0.298f, 0.686f, 0.314f);
    [SerializeField] private Color notCraftableAccent = new Color(0.333f, 0.333f, 0.333f);
    [SerializeField] private Color haveColor = new Color(0.506f, 0.784f, 0.518f);
    [SerializeField] private Color needColor = new Color(0.898f, 0.451f, 0.451f);
    [SerializeField] private Color havePillBg = new Color(0.118f, 0.239f, 0.118f);
    [SerializeField] private Color needPillBg = new Color(0.239f, 0.118f, 0.118f);

    private struct SlotData
    {
        public CraftingRecipe recipe;
        public Button button;
        public Image icon;
        public Image accentBar;
        public List<Image> pillBgs;
        public List<TextMeshProUGUI> pillTexts;
    }

    private List<SlotData> slotCache = new();
    private bool isOpen = false;

    void Start()
    {
        BuildRecipeSlots();
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
            ToggleInventory();

        if (isOpen)
            RefreshSlots();
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    void BuildRecipeSlots()
    {
        foreach (var recipe in CraftingManager.Instance.GetAllRecipes())
        {
            GameObject slot = Instantiate(recipeSlotPrefab, recipeListParent);

            Button button = slot.GetComponent<Button>();
            Image accentBar = slot.transform.Find("AccentBar").GetComponent<Image>();
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = recipe.icon;

            TextMeshProUGUI nameLabel = slot.transform.Find("Info/ItemName").GetComponent<TextMeshProUGUI>();
            nameLabel.text = recipe.itemName;

            Transform pillParent = slot.transform.Find("Info/Pills");

            foreach (Transform child in pillParent)
                Destroy(child.gameObject);

            List<Image> pillBgs = new();
            List<TextMeshProUGUI> pillTexts = new();

            foreach (var ingredient in recipe.ingredients)
            {
                GameObject pill = new GameObject("Pill", typeof(RectTransform));
                pill.transform.SetParent(pillParent, false);

                Image pillBg = pill.AddComponent<Image>();
                pillBg.color = havePillBg;

                HorizontalLayoutGroup pillLayout = pill.AddComponent<HorizontalLayoutGroup>();
                pillLayout.padding = new RectOffset(10, 10, 4, 4);
                pillLayout.childAlignment = TextAnchor.MiddleCenter;
                pillLayout.childForceExpandWidth = false;
                pillLayout.childForceExpandHeight = false;

                ContentSizeFitter pillFitter = pill.AddComponent<ContentSizeFitter>();
                pillFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                pillFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                LayoutElement pillMinSize = pill.AddComponent<LayoutElement>();
                pillMinSize.minWidth = 80;
                pillMinSize.minHeight = 24;

                GameObject textObj = new GameObject("PillText", typeof(RectTransform));
                textObj.transform.SetParent(pill.transform, false);

                TextMeshProUGUI pillText = textObj.AddComponent<TextMeshProUGUI>();
                pillText.text = $"{ingredient.amount}x {ingredient.type}";
                pillText.fontSize = 14;
                pillText.color = haveColor;
                pillText.enableWordWrapping = false;
                pillText.overflowMode = TextOverflowModes.Overflow;
                pillText.margin = new Vector4(0, 0, 0, 0);
                pillText.alignment = TextAlignmentOptions.Center;

                pillBgs.Add(pillBg);
                pillTexts.Add(pillText);
            }

            var r = recipe;
            button.onClick.AddListener(() => CraftingManager.Instance.TryCraft(r));

            slotCache.Add(new SlotData
            {
                recipe = recipe,
                button = button,
                icon = icon,
                accentBar = accentBar,
                pillBgs = pillBgs,
                pillTexts = pillTexts
            });
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(recipeListParent.GetComponent<RectTransform>());
    }

    void RefreshSlots()
    {
        foreach (var slot in slotCache)
        {
            bool craftable = CraftingManager.Instance.CanCraft(slot.recipe);

            slot.accentBar.color = craftable ? craftableAccent : notCraftableAccent;
            slot.icon.color = craftable ? Color.white : new Color(0.4f, 0.4f, 0.4f);
            slot.button.interactable = craftable;

            for (int i = 0; i < slot.recipe.ingredients.Count; i++)
            {
                var ingredient = slot.recipe.ingredients[i];
                int have = ResourceManager.Instance.GetResourceAmount(ingredient.type);
                bool hasEnough = have >= ingredient.amount;

                slot.pillTexts[i].text = $"{have}/{ingredient.amount} {ingredient.type}";
                slot.pillTexts[i].color = hasEnough ? haveColor : needColor;
                slot.pillBgs[i].color = hasEnough ? havePillBg : needPillBg;
            }
        }
    }
}