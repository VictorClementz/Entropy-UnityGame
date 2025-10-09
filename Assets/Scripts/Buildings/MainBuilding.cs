using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : Building
{
    [SerializeField] public int productionSlots = 2;
    [SerializeField] public List<ProductionRecipe> availableRecipes;

    public List<ProductionSlot> slots = new List<ProductionSlot>();
    
    public class ProductionSlot
    {
        public ProductionRecipe recipe;
        public Coroutine productionCoroutine;
    }

    void Start()
    {   
        for (int i = 0; i < productionSlots; i++)
        {
            slots.Add(new ProductionSlot());
        }
        SetRecipe(0, availableRecipes[0]); //For testing
    }
    //Call when setting a new recipe from menu
    public void SetRecipe(int slotIndex, ProductionRecipe recipe)
    {
        StopSlot(slotIndex);

        slots[slotIndex].recipe = recipe;
        slots[slotIndex].productionCoroutine = StartCoroutine(ProduceRoutine(slotIndex));
    }

    public void StopSlot(int slotIndex)
    {
        if (slots[slotIndex].productionCoroutine != null)
        {
            StopCoroutine(slots[slotIndex].productionCoroutine);
            slots[slotIndex].productionCoroutine = null;
        }
    }

    IEnumerator ProduceRoutine(int slotIndex)
    {
        ProductionSlot slot = slots[slotIndex];

        while (true)
        {
            yield return new WaitForSeconds(slot.recipe.productionTime);

            if (ResourceManager.Instance.ConsumeResources(slot.recipe))
            {
                ResourceManager.Instance.AddGold(slot.recipe.goldValue);
            }
        }
    }
    //Call when adding a new production slot
    public void AddSlot()
    {
        slots.Add(new ProductionSlot());
        productionSlots++;
    }
}