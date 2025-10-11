using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainBuildingUI : BuildingUI
{

    public MainBuilding mb;

    private Button addSlotButton;


    protected override void Awake()
    {
        base.Awake();
        panel = uiDocument.rootVisualElement.Q("MainBuildingPanel");
        panel.style.display = DisplayStyle.None;

        addSlotButton = uiDocument.rootVisualElement.Q("BuySlotButton") as Button;
        addSlotButton.RegisterCallback<ClickEvent>(CallPlaceAddSlot);
    }

    public override void ShowPanel(Building building)
    {
        base.ShowPanel(building);
        MainBuilding mainBuilding = building as MainBuilding;
        UpdateProductionSlots(mainBuilding);
       
    }


   
private void CallPlaceAddSlot(ClickEvent evt)
{
        mb.AddSlot();
  
}



    private void UpdateProductionSlots(MainBuilding building)
    {
        
    }

    protected override void UpdateDisplay()
    {
        MainBuilding mainBuilding = currentBuilding as MainBuilding;
        if (mainBuilding == null) return;

       // UpdateSlots(mainBuilding);
        //UpdateBuySlotButton(mainBuilding);
    }
}