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

    /// <summary>
    /// Set a production recipe for a specific slot
    /// </summary>
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

    /// <summary>
    /// Stop production in a specific slot (cancels current production)
    /// </summary>
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
    
    /// <summary>
    /// Production loop - consumes resources, waits, gives gold
    /// </summary>
    IEnumerator ProduceRoutine(int slotIndex)
    {
        ProductionSlot slot = slots[slotIndex];

        while (true)
        {
            // Check if can afford the recipe
            if (!ResourceManager.Instance.CanAffordRecipe(slot.recipe))
            {
               // Debug.Log($"Not enough resources for {slot.recipe.itemName}, waiting...");
                yield return new WaitForSeconds(1f);
                continue;
            }

            // Consume resources at START of production
            ResourceManager.Instance.ConsumeResources(slot.recipe);
            Debug.Log($"Started producing {slot.recipe.itemName}");

            // Wait for production time
            yield return new WaitForSeconds(slot.recipe.productionTime);

            // Give gold reward
            ResourceManager.Instance.AddGold(slot.recipe.goldValue);
            Debug.Log($"Completed {slot.recipe.itemName}, earned {slot.recipe.goldValue} gold!");
        }
    }

    /// <summary>
    /// Buy a new production slot (costs gold, gets more expensive each time)
    /// </summary>
    public bool AddSlot()
    {
        if (!ResourceManager.Instance.SpendGold(slotCost))
        {
            Debug.Log($"Not enough gold! Need {slotCost}G");
            return false;
        }

        slots.Add(new ProductionSlot());
        productionSlots++;

        // Increase cost for next slot
        slotCost = Mathf.RoundToInt(slotCost * slotCostMultiplier);

        Debug.Log($"Bought new slot! Total slots: {productionSlots}. Next slot costs: {slotCost}G");
        return true;
    }

    void OnMouseEnter()
    {
        Debug.Log($"Mouse hovering over:");
    }
}