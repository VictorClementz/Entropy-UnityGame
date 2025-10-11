using UnityEngine.UIElements;

public class MainBuildingUI : BuildingUI
{
    protected override void Awake()
    {
        base.Awake();
        panel = uiDocument.rootVisualElement.Q("MainBuildingPanel");
        panel.style.display = DisplayStyle.None;
    }

    public override void ShowPanel(Building building)
    {
        base.ShowPanel(building);
        MainBuilding mainBuilding = building as MainBuilding;
        UpdateProductionSlots(mainBuilding);
    }

    private void UpdateProductionSlots(MainBuilding building)
    {
        // Main building specific logic
    }

    protected override void UpdateDisplay()
    {
        MainBuilding mainBuilding = currentBuilding as MainBuilding;
        if (mainBuilding == null) return;

       // UpdateSlots(mainBuilding);
        //UpdateBuySlotButton(mainBuilding);
    }
}