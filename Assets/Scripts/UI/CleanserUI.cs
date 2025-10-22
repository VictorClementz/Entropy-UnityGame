using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CleanserUI : BuildingUI
{
    private Button focusedModeButton;
    private Button areaModeButton;
    private Label statusLabel;
    private bool waitingForDestination = false;
    private Button setDestinationButton;

    protected override void Awake()
    {
        

        base.Awake();

      

        panel = uiDocument.rootVisualElement.Q("CleanserPanel");
        panel.style.display = DisplayStyle.None;

        focusedModeButton = uiDocument.rootVisualElement.Q<Button>("FocusedModeButton");
        areaModeButton = uiDocument.rootVisualElement.Q<Button>("AreaModeButton");
        statusLabel = uiDocument.rootVisualElement.Q<Label>("CleanserStatusLabel");
        setDestinationButton = uiDocument.rootVisualElement.Q<Button>("setDestinationButton");

        focusedModeButton.RegisterCallback<ClickEvent>(evt => OnModeButtonClicked(Cleanser.CleanseMode.Focused));
        areaModeButton.RegisterCallback<ClickEvent>(evt => OnModeButtonClicked(Cleanser.CleanseMode.Area));
        setDestinationButton.RegisterCallback<ClickEvent>(evt => OnSetDestinationClicked());

        Debug.Log($"focusedModeButton found: {focusedModeButton != null}");
        Debug.Log($"areaModeButton found: {areaModeButton != null}");
        Debug.Log($"setDestinationButton found: {setDestinationButton != null}");
    }

    public override void ShowPanel(Building building)
    {
        base.ShowPanel(building);
        UpdateDisplay();
    }

    void Update()
    {
        //Handle destination selection
        if (waitingForDestination && currentBuilding != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2Int selectedTile = GridManager.Instance.GetSelectedTile();

                if (selectedTile.x != -1)
                {
                    Cleanser cleanser = currentBuilding as Cleanser;
                    cleanser.SetDestination(selectedTile.x, selectedTile.y);
                    waitingForDestination = false;
                    UpdateDisplay();
                }
            }

            // Cancel with right click
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                waitingForDestination = false;
                UpdateDisplay();
            }
        }
    }


    protected override void UpdateDisplay()
    {
        Cleanser cleanser = currentBuilding as Cleanser;
        if (cleanser == null) return;

        //Update status label
        if (statusLabel != null)
        {
            statusLabel.text = $"Mode: {cleanser.GetMode()}";
        }

        if (statusLabel != null)
        {
            if (waitingForDestination)
            {
                statusLabel.text = "Click a tile to set destination";
            }
            else if (cleanser.IsMoving())
            {
                statusLabel.text = "Moving...";
            }
            else
            {
                statusLabel.text = $"Mode: {cleanser.GetMode()}";
            }
        }

        //Highlight active mode button
        UpdateModeButtons(cleanser.GetMode());
    }

    void OnSetDestinationClicked() 
    {
        waitingForDestination = true;
        Debug.Log("WAITING FOR DESTINATION");
        UpdateDisplay();
    }

    void OnModeButtonClicked(Cleanser.CleanseMode mode)
    {
        Cleanser cleanser = currentBuilding as Cleanser;
        if (cleanser == null) return;

        cleanser.SetMode(mode);
        UpdateDisplay();
    }

    void UpdateModeButtons(Cleanser.CleanseMode currentMode)
    {
        //Visual feedback
        if (focusedModeButton != null)
        {
            if (currentMode == Cleanser.CleanseMode.Focused)
            {
                focusedModeButton.style.backgroundColor = new StyleColor(new Color(0.2f, 0.8f, 0.2f));
            }
            else
            {
                focusedModeButton.style.backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            }
        }

        if (areaModeButton != null)
        {
            if (currentMode == Cleanser.CleanseMode.Area)
            {
                areaModeButton.style.backgroundColor = new StyleColor(new Color(0.2f, 0.8f, 0.2f));
            }
            else
            {
                areaModeButton.style.backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            }
        }
    }
}