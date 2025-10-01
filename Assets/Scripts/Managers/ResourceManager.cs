
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    

    public event System.Action<int> OnGoldChanged;
    public event System.Action<ResourceType, int> OnResourceChanged;

    [Header("Current Resources")]
    public int gold = 100;
    public int wood = 0;
    public int stone = 0;
    public int iron = 0;
    public int oil = 0;
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    //Call when adding resources
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.Stone:
                stone += amount;
                break;
            case ResourceType.Iron:
                iron += amount;
                break;
            case ResourceType.Oil:
                oil += amount;
                break;
        }

        Debug.Log($"Added {amount} {type}. Total: {GetResource(type)}");
    }

    public int GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood: return wood;
            case ResourceType.Stone: return stone;
            case ResourceType.Iron: return iron;
            case ResourceType.Oil: return oil;
            default: return 0;
        }
    }

    //Call when adding gold, I could add that to addResource
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
        
    }

    //Call when spending cold
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        return false;
    }


    public int GetGold() => gold;

   
    public bool CanAffordRecipe(ProductionRecipe recipe)
    {
        foreach (var cost in recipe.costs)
        {
            if (GetResource(cost.resourceType) < cost.amount)
                return false;
        }
        return true;
    }

    //Call when removing resource
    public bool ConsumeResources(ProductionRecipe recipe)
    {
        if (!CanAffordRecipe(recipe))
            return false;

        foreach (var cost in recipe.costs)
        {
            
            switch (cost.resourceType)
            {
                case ResourceType.Wood:
                    wood -= cost.amount;
                    break;
                case ResourceType.Stone:
                    stone -= cost.amount;
                    break;
                case ResourceType.Iron:
                    iron -= cost.amount;
                    break;
                case ResourceType.Oil:
                    oil -= cost.amount;
                    break;
            }

            OnResourceChanged?.Invoke(cost.resourceType, GetResource(cost.resourceType));
        }

        
        return true;
    }

}