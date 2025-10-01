using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScript : MonoBehaviour
{
    private UIDocument uIDocument;

    private Button resButton;
    private Button mainButton;

    [SerializeField] private BuildingPlacer buildingPlacer;
    void Awake()
    {
        uIDocument = GetComponent<UIDocument>();

        resButton = uIDocument.rootVisualElement.Q("ResButton") as Button;
        resButton.RegisterCallback<ClickEvent>(CallPlaceBuilding);
    }

    private void CallPlaceBuilding(ClickEvent evt)
    {
      //  resButton.UnregisterCallback<ClickEvent>(CallPlaceBuilding);
        buildingPlacer.PlaceBuilding(0); //Plays resources for now
        Debug.Log("Place buidling");
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
