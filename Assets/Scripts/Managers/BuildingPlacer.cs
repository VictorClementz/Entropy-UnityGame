using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject[] buildingPrefabs;
    private int selectedBuildingIndex = 0;

    
    public void PlaceBuilding(int selectedBuildingIndex)
    {
        Vector2Int selectedTile = gridManager.GetSelectedTile();
        Debug.Log("1");
        if (selectedTile.x == -1) return; //No selected tile
        Debug.Log("2");
        if (!gridManager.ValidPlacement(selectedTile.x, selectedTile.y)) return;
        Debug.Log("3");
        Vector3 worldPos = new Vector3(selectedTile.x, 0.5f, selectedTile.y);
        GameObject building = Instantiate(buildingPrefabs[selectedBuildingIndex], worldPos, Quaternion.identity);
        Debug.Log("4");
        gridManager.SetTileOccupied(selectedTile.x, selectedTile.y, building);
    }
}