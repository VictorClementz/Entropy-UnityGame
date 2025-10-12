using System.Collections.Generic;
using UnityEngine;

public class InfluenceZone : MonoBehaviour
{
    [SerializeField] private int radius = 1;

    void Start()
    {
        // Register this zone
        if (GridManager.Instance != null)
        {
            GridManager.Instance.RegisterInfluenceZone(this);
        }

        RefreshBorders();
    }

    void OnDestroy()
    {
        // Unregister this zone
        if (GridManager.Instance != null)
        {
            GridManager.Instance.UnregisterInfluenceZone(this);
        }

        RefreshBorders();
    }

    void RefreshBorders()
    {
        if (InfluenceBorderManager.Instance != null)
        {
            InfluenceBorderManager.Instance.RefreshBorder();
        }
    }

    public bool IsPositionInZone(int x, int y)
    {
        Vector3 buildingPos = transform.position;
        int centerX = Mathf.RoundToInt(buildingPos.x);
        int centerY = Mathf.RoundToInt(buildingPos.z);

        int distance = Mathf.Max(Mathf.Abs(x - centerX), Mathf.Abs(y - centerY));
        return distance <= radius;
    }

    public List<Vector2Int> GetCellsInZone()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        Vector3 buildingPos = transform.position;
        int centerX = Mathf.RoundToInt(buildingPos.x);
        int centerY = Mathf.RoundToInt(buildingPos.z);

        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                cells.Add(new Vector2Int(x, y));
            }
        }

        return cells;
    }

    public int GetRadius() => radius;
}