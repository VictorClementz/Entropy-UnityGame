using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected string buildingName;

    protected GridCell2 gridCell;

    protected GridManager gridManager;
    [SerializeField] public int cost;
    private Mouse mouse;
    private Camera mainCamera;


    protected virtual void Awake()
    {

        gridManager = GridManager.Instance;

        if (gridManager == null)
        {
            
        }

        mouse = Mouse.current;
        mainCamera = Camera.main;
    }
    public virtual void OnPlaced(GridCell2 cell)
    {
        gridCell = cell;
        Debug.Log($"{buildingName} placed at ({cell.x}, {cell.y})");
 
    }

    void Update()
    {
        
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
           

                if (hit.collider.gameObject == this.gameObject)
                {
                    OnBuildingClicked();
                }
            }
        }
    }

    void OnBuildingClicked()
    {
       

        if (BuildingUIManager.Instance != null)
        {
            BuildingUIManager.Instance.ShowBuildingUI(this);
        }
        else
        {
         
        }
    }


}