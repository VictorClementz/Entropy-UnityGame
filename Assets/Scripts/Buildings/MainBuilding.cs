using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : Building
{
    [SerializeField] public int productionSlots = 2;
    [SerializeField] public List<ProductionRecipe> availableRecipes;
    [SerializeField] public int slotCost = 50;
    [SerializeField] private float slotCostMultiplier = 1.5f;

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

        // For testing - auto-start first slot
        if (availableRecipes.Count > 0)
        {
            SetRecipe(0, availableRecipes[0]);
        }
    }

    
    public void SetRecipe(int slotIndex, ProductionRecipe recipe)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        StopSlot(slotIndex);
        slots[slotIndex].recipe = recipe;
        slots[slotIndex].productionCoroutine = StartCoroutine(ProduceRoutine(slotIndex));

        Debug.Log($"Slot {slotIndex} now producing {recipe.itemName}");
    }

   
    public void StopSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        if (slots[slotIndex].productionCoroutine != null)
        {
            StopCoroutine(slots[slotIndex].productionCoroutine);
            slots[slotIndex].productionCoroutine = null;
        }

        slots[slotIndex].recipe = null;
    }
    
   
    IEnumerator ProduceRoutine(int slotIndex)
    {
        ProductionSlot slot = slots[slotIndex];

        while (true)
        {
          
            if (!ResourceManager.Instance.CanAffordRecipe(slot.recipe))
            {
               
                yield return new WaitForSeconds(1f);
                continue;
            }

            
            ResourceManager.Instance.ConsumeResources(slot.recipe);
        

     
            yield return new WaitForSeconds(slot.recipe.productionTime);

           
            ResourceManager.Instance.AddGold(slot.recipe.goldValue);
           
        }
    }

    /// <summary>
    /// Buy a new production slot (costs gold, gets more expensive each time)
    /// </summary>
    public bool AddSlot()
    {
        if (!ResourceManager.Instance.SpendGold(slotCost))
        {
           
            return false;
        }

        slots.Add(new ProductionSlot());
        productionSlots++;

        // Increase cost for next slot
        slotCost = Mathf.RoundToInt(slotCost * slotCostMultiplier);

        Debug.Log("new slot added");
        return true;
    }

  
}