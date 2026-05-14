using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [System.Serializable]
    public struct Ingredient
    {
        public ResourceType type;
        public int amount;
    }

    public List<Ingredient> ingredients;
    public ResourceType outputType; // what resource crafting this produces
    public int outputAmount;
}