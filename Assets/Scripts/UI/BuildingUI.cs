using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Abstract base class for all building UI panels
/// Handles showing/hiding and provides common functionality
/// </summary>
public abstract class BuildingUI : MonoBehaviour
{
    protected UIDocument uiDocument;
    protected VisualElement panel;
    protected Building currentBuilding;

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    /// <summary>
    /// Show this building's UI panel
    /// Override to add building-specific logic
    /// </summary>
    public virtual void ShowPanel(Building building)
    {
        if (panel == null)
        {
            Debug.LogError($"Panel not set for {GetType().Name}!");
            return;
        }

        currentBuilding = building;
        panel.style.display = DisplayStyle.Flex;
        UpdateDisplay();
    }

    /// <summary>
    /// Hide this building's UI panel
    /// </summary>
    public virtual void HidePanel()
    {
        if (panel != null)
        {
            panel.style.display = DisplayStyle.None;
        }
        currentBuilding = null;
    }

    /// <summary>
    /// Update the UI display with current building data
    /// Override in child classes to implement specific UI updates
    /// </summary>
    protected virtual void UpdateDisplay(){

}


    /// <summary>
    /// Called when panel needs to refresh (e.g., after a purchase)
    /// </summary>
    public void RefreshDisplay()
    {
        if (currentBuilding != null)
        {
            UpdateDisplay();
        }
    }
}