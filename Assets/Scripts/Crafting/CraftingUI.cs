using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform recipeListParent;

    [Header("Colors")]
    [SerializeField] private Color craftableColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color notCraftableColor = new Color(0.4f, 0.4f, 0.4f);

    private List<(CraftingRecipe recipe, Button button, Image icon)> slotCache = new();
    private bool isOpen = false;

    void Start()
    {
        BuildRecipeSlots();
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();

        if (isOpen)
            RefreshSlots(); // updates green/gray each frame while open
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

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI label = slot.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            Button button = slot.GetComponent<Button>();

            icon.sprite = recipe.icon;
            label.text = recipe.itemName;

            // capture recipe in closure for the onclick
            var r = recipe;
            button.onClick.AddListener(() => CraftingManager.Instance.TryCraft(r));

            slotCache.Add((recipe, button, icon));
        }
    }

    void RefreshSlots()
    {
        foreach (var (recipe, button, icon) in slotCache)
        {
            bool craftable = CraftingManager.Instance.CanCraft(recipe);
            icon.color = craftable ? craftableColor : notCraftableColor;
            button.interactable = craftable;
        }
    }
}