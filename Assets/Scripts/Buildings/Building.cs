using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected string buildingName;

    protected GridCell2 gridCell;

    protected GridManager gridManager;
    [SerializeField] public int cost;


    protected virtual void Awake()
    {

        gridManager = GridManager.Instance;

        if (gridManager == null)
        {
            Debug.LogError($"GridManager not found for {gameObject.name}!");
        }
    }
    public virtual void OnPlaced(GridCell2 cell)
    {
        gridCell = cell;
        Debug.Log($"{buildingName} placed at ({cell.x}, {cell.y})");
 
    }

    void OnMouseDown()
    {
        Debug.Log($"Building clicked: {buildingName}"); // ADD THIS LINE

        if (BuildingUIManager.Instance != null)
        {
            BuildingUIManager.Instance.ShowBuildingUI(this);
        }
        else
        {
            Debug.LogError("BuildingUIManager not found in scene!");
        }
    }

    
}