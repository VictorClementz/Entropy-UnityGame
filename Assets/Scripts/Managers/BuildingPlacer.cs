using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject[] buildingPrefabs;
    
    private int selectedBuildingIndex = 0;

    
    public void PlaceBuilding(int selectedBuildingIndex)
    {
        Vector2Int selectedTile = gridManager.GetSelectedTile();
        if (selectedTile.x == -1) return; //No selected tile

        Building buildingComponent = buildingPrefabs[selectedBuildingIndex].GetComponent<Building>();

        if (buildingComponent == null) //redundancy
        {
            Debug.LogError("Prefab doesn't have a Building component!");
            return;
        }

        if (buildingComponent.cost > ResourceManager.Instance.GetGold()) // check if player has enough gold
        {
            return;
        }
        ResourceManager.Instance.SpendGold(buildingComponent.cost); //spend gokld

        Vector3 worldPos = new Vector3(selectedTile.x, 0.5f, selectedTile.y);

        GameObject building = Instantiate(buildingPrefabs[selectedBuildingIndex], worldPos, Quaternion.identity);; //spawn building
        gridManager.SetTileOccupied(selectedTile.x, selectedTile.y, building); //Update cell
    }
}