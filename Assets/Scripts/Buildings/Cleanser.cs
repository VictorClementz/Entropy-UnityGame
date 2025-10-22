
using UnityEngine;
using UnityEngine.InputSystem;

public class Cleanser : Building
{
    [Header("Cleansing Settings")]
    [SerializeField] private float cleanseRate = 0.2f; 
    [SerializeField] private CleanseMode currentMode = CleanseMode.Focused;

    //private GridManager gridManager;
    private Vector2Int currentPosition;
    private bool isActive = true;

    public enum CleanseMode
    {
        Focused,  
        Area     
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Cleanser Awake called");
        gridManager = GridManager.Instance;
    }

    void Start()
    {
        


    }

    void Update()
    {
        base.Update();

        currentPosition = new Vector2Int(
           Mathf.RoundToInt(transform.position.x),
           Mathf.RoundToInt(transform.position.z)
       );
        if (!isActive) return;

        if (currentMode == CleanseMode.Focused)
        {
            CleanseFocused();
        }
        else if (currentMode == CleanseMode.Area)
        {
            CleanseArea();
        }
    }

    void CleanseFocused()
    {
        GridCell2 cell = gridManager.GetCell(currentPosition.x, currentPosition.y);
        
        if (cell != null && cell.isCorrupted)
        {
            CleanseCell(cell);
            Debug.Log(cell.corruptionLevel);
        }
    }

    void CleanseArea()  
    {
        //Cleanse 3x3 area around cleanser
        for (int x = currentPosition.x - 1; x <= currentPosition.x + 1; x++)
        {
            for (int y = currentPosition.y - 1; y <= currentPosition.y + 1; y++)
            {
                GridCell2 cell = gridManager.GetCell(x, y);

                if (cell != null && cell.isCorrupted)
                {
                    //Cleanse time reduction
                    CleanseCell(cell, cleanseRate * 0.3f);
                    
                }
            }
        }
    }

    void CleanseCell(GridCell2 cell, float rate = -1)
    {
        if (rate < 0) rate = cleanseRate;

        //Reduce corruption level
        cell.corruptionLevel -= rate * Time.deltaTime;
        Debug.Log("Cleansing" + cell);
        if (cell.corruptionLevel <= 0f)
        {
            cell.corruptionLevel = 0f;
            cell.isCorrupted = false;
           

            CorruptionManager.Instance.RemoveCorruptedCell(cell.x, cell.y);
        }

        gridManager.UpdateTileVisual(cell.x, cell.y);
    }

    public void SetMode(CleanseMode mode)
    {
        currentMode = mode;
       
    }

    public CleanseMode GetMode()
    {
        return currentMode;
    }
}