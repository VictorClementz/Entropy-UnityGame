using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainBuildingUI : BuildingUI
{
    private Button addSlotButton;
    private Button prodSlot1;
    private Button prodSlot2;
    private VisualElement productionSelectionPanel;
    private Button confirmRecipeBtn;
    private Button cancelRecipeBtn;
    private VisualElement recipeButtonsContainer;

    private Button recipeButton1;
    private Button recipeButton2;
    private Button recipeButton3;

    private int currentSlotIndex = -1;
    private List<Button> productionSlotButtons = new List<Button>();



    protected override void Awake()
    {
        base.Awake();

        panel = uiDocument.rootVisualElement.Q("MainBuildingPanel");
        panel.style.display = DisplayStyle.None;

        
        addSlotButton = uiDocument.rootVisualElement.Q<Button>("BuySlotButton");
        prodSlot1 = uiDocument.rootVisualElement.Q<Button>("ProdSlot1");
        prodSlot2 = uiDocument.rootVisualElement.Q<Button>("ProdSlot2");

        recipeButton1 = uiDocument.rootVisualElement.Q<Button>("RecipeButton1");
        recipeButton2 = uiDocument.rootVisualElement.Q<Button>("RecipeButton2");
        recipeButton3 = uiDocument.rootVisualElement.Q<Button>("RecipeButton3");



        addSlotButton.RegisterCallback<ClickEvent>(CallPlaceAddSlot);


        
            prodSlot1.RegisterCallback<ClickEvent>(evt => OnProdSlot1Clicked(0));
        prodSlot2.RegisterCallback<ClickEvent>(evt => OnProdSlot1Clicked(1));

        recipeButton1.RegisterCallback<ClickEvent>(evt => AddRecipeToProd(0));
        recipeButton2.RegisterCallback<ClickEvent>(evt => AddRecipeToProd(1));
        recipeButton3   .RegisterCallback<ClickEvent>(evt => AddRecipeToProd(2));



        productionSelectionPanel = uiDocument.rootVisualElement.Q("RecipeSelectionPanel");

        recipeButtonsContainer = uiDocument.rootVisualElement.Q("RecipeButtonsContainer");
        cancelRecipeBtn = uiDocument.rootVisualElement.Q<Button>("CancelRecipeBtn");




    }

    public override void ShowPanel(Building building)
    {
        base.ShowPanel(building);
        MainBuilding mainBuilding = building as MainBuilding;


        UpdateProductionSlots(mainBuilding);
    }

    private void CallPlaceAddSlot(ClickEvent evt)
    {
        MainBuilding mainBuilding = currentBuilding as MainBuilding;

       
            mainBuilding.AddSlot();
            UpdateDisplay(); 
       
    }

    private void OnProdSlot1Clicked(int slotIndex)
    {
        currentSlotIndex = slotIndex;
        ShowProductionList();
    }

    private void ShowProductionList()
    {
        if (productionSelectionPanel != null)
        {
            productionSelectionPanel.style.display = DisplayStyle.Flex;
          
        }
    }

    private void UpdateProductionSlots(MainBuilding building)
    {
        ;
    }

    protected override void UpdateDisplay()
    {
        MainBuilding mainBuilding = currentBuilding as MainBuilding;
        if (mainBuilding == null) return;

        // UpdateSlots(mainBuilding);
        // UpdateBuySlotButton(mainBuilding);
    }

    private void AddRecipeToProd(int recipeSlot)
    {

        Debug.Log(currentSlotIndex);
        MainBuilding mainBuilding = currentBuilding as MainBuilding;
        List<ProductionRecipe> recipes = mainBuilding.GetAvailableRecipes();


        mainBuilding.SetRecipe(currentSlotIndex, recipes[recipeSlot]);
        
    }
}