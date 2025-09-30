using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//Recipies for MainBuilding prod
public class ResourceCost
{
    public ResourceType resourceType;
    public int amount;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Production/Recipe")]
public class ProductionRecipe : ScriptableObject
{
    public string itemName; 
    public List<ResourceCost> costs; 
    public int goldValue; 
    public float productionTime; 
   // public Sprite icon; 
}