using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    public static BuildingUIManager Instance;

    [SerializeField] private MainBuildingUI mainBuildingUI;
   // [SerializeField] private ResourceBuildingUI resourceBuildingUI;
    // Add more as needed

    private BuildingUI currentOpenUI;

    void Awake()
    {
        Instance = this;
    }

    public void ShowBuildingUI(Building building)
    {
        // Close any open UI first
        HideCurrentUI();

        // Show correct UI based on building type
        if (building is MainBuilding mainBuilding)
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