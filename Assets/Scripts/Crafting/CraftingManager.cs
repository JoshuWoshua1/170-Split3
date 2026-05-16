using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [SerializeField] private List<CraftingRecipe> allRecipes;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (ResourceManager.Instance.GetResourceAmount(ingredient.type) < ingredient.amount)
                return false;
        }
        return true;
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
        {
            //Debug.Log($"Cannot craft {recipe.itemName}: insufficient resources.");
            return false;
        }

        foreach (var ingredient in recipe.ingredients)
            ResourceManager.Instance.RemoveResource(ingredient.type, ingredient.amount);

        ResourceManager.Instance.AddResource(recipe.outputType, recipe.outputAmount);
        GameManager.Instance.AddCraftedCount(recipe.outputAmount);
        //Debug.Log($"Crafted {recipe.itemName}. +{recipe.outputAmount} {recipe.outputType}.");
        return true;
    }

    public List<CraftingRecipe> GetAllRecipes() => allRecipes;
}