using System;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScript : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button resButton;
    private Button mainButton;

    private Label goldLabel;
    private Label woodLabel;
    private Label stoneLabel;
    private Label ironLabel;
    private Label oilLabel;

    [SerializeField] private BuildingPlacer buildingPlacer;
    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;
        root.pickingMode = PickingMode.Ignore;

        resButton = uiDocument.rootVisualElement.Q("ResButton") as Button;
        resButton.RegisterCallback<ClickEvent>(CallPlaceBuilding);
//topbar
        

        goldLabel = root.Q<Label>("GoldLabel");
        woodLabel = root.Q<Label>("WoodLabel");
        stoneLabel = root.Q<Label>("StoneLabel");
        ironLabel = root.Q<Label>("IronLabel");
        oilLabel = root.Q<Label>("OilLabel");
    }
   //test
    /// <param name="evt"></param>
    private void CallPlaceBuilding(ClickEvent evt)
    {
      //  resButton.UnregisterCallback<ClickEvent>(CallPlaceBuilding);
        buildingPlacer.PlaceBuilding(0); //Places resources for now
        Debug.Log("Place buidling");
    }

    ///////////HEADS UP////////////////
    //OnEnable runs before resourcemanager Awake() so instead of fixing it i just changed the execution order so resourceManger is the one of the first scripts to run

    void OnEnable() 
    {
        
        ///Subsribe to update
        ResourceManager.Instance.OnGoldChanged += UpdateGold;           
        ResourceManager.Instance.OnResourceChanged += UpdateAllResource;

        
        UpdateResourceDisplay();
    }

    private void UpdateGold(int newAmount) //Same as in resource manager, could add gold as a general resource but i kinda like to keep it seperate
    {
        goldLabel.text = $"Gold: {newAmount}";
    }

    private void UpdateAllResource(ResourceType type, int newAmount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                woodLabel.text = $"Wood: {newAmount}";
                break;
            case ResourceType.Stone:
                stoneLabel.text = $"Stone: {newAmount}";
                break;
            case ResourceType.Iron:
                ironLabel.text = $"Iron: {newAmount}";
                break;
            case ResourceType.Oil:
                oilLabel.text = $"Oil: {newAmount}";
                break;
            default:
                return;
        }
    }

    public void UpdateResourceDisplay()
    {
        
        goldLabel.text = $"Gold: {ResourceManager.Instance.gold}"; //Realized that have been incosnistant with gold or Gold, fix
        woodLabel.text = $"Wood: {ResourceManager.Instance.wood}"; 
        stoneLabel.text = $"Stone: {ResourceManager.Instance.stone}";
        ironLabel.text = $"Iron: {ResourceManager.Instance.iron}";
        oilLabel.text = $"Oil: {ResourceManager.Instance.oil}";
    }
}

