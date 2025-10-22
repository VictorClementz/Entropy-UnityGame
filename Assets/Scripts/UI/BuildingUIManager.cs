using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    public static BuildingUIManager Instance;

    [SerializeField] private MainBuildingUI mainBuildingUI;
    [SerializeField] private CleanserUI cleanserUI;

    // [SerializeField] private ResourceBuildingUI resourceBuildingUI;
    

    private BuildingUI currentOpenUI;

    void Awake()
    {
        Instance = this;
    }

    public void ShowBuildingUI(Building building)
    {
        // Close any open UI first
        HideCurrentUI();

        if (building is Cleanser) // Add this
        {
            cleanserUI.ShowPanel(building);
            currentOpenUI = cleanserUI;
            Debug.Log("1");
            
        }
        else if (building is MainBuilding mainBuilding)
        {
            mainBuildingUI.ShowPanel(mainBuilding);
            currentOpenUI = mainBuildingUI;
        }
       
        //else if (building is ResourceBuilding resBuilding)
        //{
        //     resourceBuildingUI.ShowPanel(resBuilding);
        //    currentOpenUI = resourceBuildingUI;
        //  }
    }

    public void HideCurrentUI()
    {
        if (currentOpenUI != null)
        {
            currentOpenUI.HidePanel();
            currentOpenUI = null;
        }
    }
}