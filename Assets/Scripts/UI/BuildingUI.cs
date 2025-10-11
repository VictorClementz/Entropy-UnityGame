using UnityEngine;
using UnityEngine.UIElements;


public abstract class BuildingUI : MonoBehaviour
{
    protected UIDocument uiDocument;
    protected VisualElement panel;
    protected Building currentBuilding;

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

   
    public virtual void ShowPanel(Building building)
    {
        if (panel == null)
        {
            Debug.LogError($"Panel not set for {GetType().Name}!");
            return;
        }

        //Show the right sidebar first
        var sidebarRight = uiDocument.rootVisualElement.Q("SidebarRight");
        if (sidebarRight != null)
            sidebarRight.style.display = DisplayStyle.Flex;

        currentBuilding = building;
        panel.style.display = DisplayStyle.Flex;
        UpdateDisplay();
    }

    public virtual void HidePanel()
    {
        if (panel != null)
        {
            panel.style.display = DisplayStyle.None;
        }

       
        var sidebarRight = uiDocument.rootVisualElement.Q("SidebarRight");
        if (sidebarRight != null)
            sidebarRight.style.display = DisplayStyle.None;

        currentBuilding = null;
    }

   
    protected virtual void UpdateDisplay(){

}


   
    public void RefreshDisplay()
    {
        if (currentBuilding != null)
        {
            UpdateDisplay();
        }
    }
}