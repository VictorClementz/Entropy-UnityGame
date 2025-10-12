using System.Collections.Generic;
using UnityEngine;

public class Outpost : MainBuilding
{
    protected override void Awake()
    {
        base.Awake(); 

        
        productionSlots = 1;

        if (availableRecipes == null || availableRecipes.Count == 0)
        {
            // Find MainBuilding in scene to copy its recipes
            MainBuilding mainBuilding = FindFirstObjectByType<MainBuilding>();
            if (mainBuilding != null && mainBuilding != this)
            {
                availableRecipes = new List<ProductionRecipe>(mainBuilding.availableRecipes);
            }
        }
    }

    void Start()
    {
        
        slots.Clear(); //clear inheratance

        for (int i = 0; i < productionSlots; i++)
        {
            slots.Add(new ProductionSlot());
        }
    }
}