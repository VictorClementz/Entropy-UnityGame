using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : Building
{
    [SerializeField] protected int productionSlots = 2;
    public List<ProductionRecipe> availableRecipes = new List<ProductionRecipe>();
    [SerializeField] public int slotCost = 50;
    [SerializeField] private float slotCostMultiplier = 1.5f;

    public List<ProductionSlot> slots = new List<ProductionSlot>();
    private InfluenceZone influenceZone;

    public class ProductionSlot
    {
        public ProductionRecipe recipe;
        public Coroutine productionCoroutine;
    }

    protected override void Awake() 
    {
        base.Awake(); //Call Building's Awake
                      
    }

    void Start()
    {
        influenceZone = GetComponent<InfluenceZone>();
        for (int i = 0; i < productionSlots; i++)
        {
            slots.Add(new ProductionSlot());
        }
    }

    
    public bool IsPositionInInfluence(int x, int y)
    {
        return influenceZone != null && influenceZone.IsPositionInZone(x, y);
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

    public bool AddSlot()
    {
        if (!ResourceManager.Instance.SpendGold(slotCost))
        {
            return false;
        }
        slots.Add(new ProductionSlot());
        productionSlots++;
        slotCost = Mathf.RoundToInt(slotCost * slotCostMultiplier);
        Debug.Log("new slot added");
        return true;
    }

    public int GetSlotCount()
    {
        return slots.Count;
    }

    public List<ProductionRecipe> GetAvailableRecipes()
    {
        return availableRecipes;
    }
}