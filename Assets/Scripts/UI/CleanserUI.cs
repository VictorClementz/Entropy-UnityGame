using UnityEngine;
using UnityEngine.UIElements;

public class CleanserUI : BuildingUI
{
    private Button focusedModeButton;
    private Button areaModeButton;
    private Label statusLabel;

    protected override void Awake()
    {
        base.Awake();

        panel = uiDocument.rootVisualElement.Q("CleanserPanel");
        panel.style.display = DisplayStyle.None;

        focusedModeButton = uiDocument.rootVisualElement.Q<Button>("FocusedModeButton");
        areaModeButton = uiDocument.rootVisualElement.Q<Button>("AreaModeButton");
        statusLabel = uiDocument.rootVisualElement.Q<Label>("CleanserStatusLabel");

        focusedModeButton.RegisterCallback<ClickEvent>(evt => OnModeButtonClicked(Cleanser.CleanseMode.Focused));
        areaModeButton.RegisterCallback<ClickEvent>(evt => OnModeButtonClicked(Cleanser.CleanseMode.Area));
    }

    public override void ShowPanel(Building building)
    {
        base.ShowPanel(building);
        UpdateDisplay();
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

        //Highlight active mode button
        UpdateModeButtons(cleanser.GetMode());
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