using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform recipeListParent;

    [Header("Colors")]
    [SerializeField] private Color craftableAccent = new Color(0.298f, 0.686f, 0.314f);   // green border
    [SerializeField] private Color notCraftableAccent = new Color(0.333f, 0.333f, 0.333f); // gray border
    [SerializeField] private Color haveColor = new Color(0.506f, 0.784f, 0.518f);          // green pill text
    [SerializeField] private Color needColor = new Color(0.898f, 0.451f, 0.451f);          // red pill text
    [SerializeField] private Color havePillBg = new Color(0.118f, 0.239f, 0.118f);         // dark green pill bg
    [SerializeField] private Color needPillBg = new Color(0.239f, 0.118f, 0.118f);         // dark red pill bg

    private struct SlotData
    {
        public CraftingRecipe recipe;
        public Button button;
        public Image icon;
        public Image accentBar;         // left colored bar
        public List<Image> pillBgs;     // ingredient pill backgrounds
        public List<TextMeshProUGUI> pillTexts; // ingredient pill labels
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

            // --- root references ---
            Button button = slot.GetComponent<Button>();
            Image accentBar = slot.transform.Find("AccentBar").GetComponent<Image>();

            // --- icon (circle) ---
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = recipe.icon;

            // --- name label ---
            TextMeshProUGUI nameLabel = slot.transform.Find("Info/ItemName").GetComponent<TextMeshProUGUI>();
            nameLabel.text = recipe.itemName;

            // --- ingredient pills ---
            Transform pillParent = slot.transform.Find("Info/Pills");

            // clear any placeholder pills in the prefab
            foreach (Transform child in pillParent)
                Destroy(child.gameObject);

            List<Image> pillBgs = new();
            List<TextMeshProUGUI> pillTexts = new();

            foreach (var ingredient in recipe.ingredients)
            {
                GameObject pill = new GameObject("Pill", typeof(RectTransform));
                pill.transform.SetParent(pillParent, false);

                // pill background image
                Image pillBg = pill.AddComponent<Image>();
                pillBg.color = havePillBg;

                // pill layout
                HorizontalLayoutGroup pillLayout = pill.AddComponent<HorizontalLayoutGroup>();
                pillLayout.padding = new RectOffset(6, 6, 2, 2);
                pillLayout.childAlignment = TextAnchor.MiddleCenter;
                pillLayout.childForceExpandWidth = false;
                pillLayout.childForceExpandHeight = false;

                ContentSizeFitter pillFitter = pill.AddComponent<ContentSizeFitter>();
                pillFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                pillFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                // set corner radius via outline (Unity doesn't have native rounded rects —
                // use a rounded sprite for pillBg.sprite if you want true rounded corners)

                // pill text: "3x Water"
                GameObject textObj = new GameObject("PillText", typeof(RectTransform));
                textObj.transform.SetParent(pill.transform, false);

                TextMeshProUGUI pillText = textObj.AddComponent<TextMeshProUGUI>();
                pillText.text = $"{ingredient.amount}x {ingredient.type}";
                pillText.fontSize = 10;
                pillText.color = haveColor;

                pillBgs.Add(pillBg);
                pillTexts.Add(pillText);
            }

            // --- craft button ---
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
    }

    void RefreshSlots()
    {
        foreach (var slot in slotCache)
        {
            bool craftable = CraftingManager.Instance.CanCraft(slot.recipe);

            // update accent bar color
            slot.accentBar.color = craftable ? craftableAccent : notCraftableAccent;

            // dim icon when not craftable
            slot.icon.color = craftable ? Color.white : new Color(0.4f, 0.4f, 0.4f);

            // update button
            slot.button.interactable = craftable;

            // update each ingredient pill individually
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