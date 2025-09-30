using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected string buildingName;

    protected GridCell2 gridCell;

    protected GridManager gridManager;


    protected virtual void Awake()
    {

        gridManager = FindAnyObjectByType<GridManager>();

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
}