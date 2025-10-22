using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected string buildingName;

    protected GridCell2 gridCell;

    protected GridManager gridManager;
    [SerializeField] public int cost;
    public Mouse mouse;
    public Camera mainCamera;


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

   public void Update()
    {
        
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                Debug.Log("cast");
                if (hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("hit");
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