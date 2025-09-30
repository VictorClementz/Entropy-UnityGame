
using UnityEngine;

//Data for indvidual Cells/Tiles
//I need to be more consistant with cells/tiles

//TODO Change all Gridcell2 -> Gridcell
[System.Serializable]
public class GridCell2
{
    public int x, y;
    public bool isOccupied;
    public GameObject building;
    public ResourceType resourceType;



    public GridCell2(int x, int y, bool isOccupied)
    {
        this.x = x;
        this.y = y;
        this.isOccupied = false;
        this.resourceType = ResourceType.None;
    }
}