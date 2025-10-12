using System.Collections.Generic;
using UnityEngine;

public class InfluenceBorderManager : MonoBehaviour
{
    public static InfluenceBorderManager Instance;

    [SerializeField] private Color borderColor = Color.green;

    private GameObject borderObject;
    private LineRenderer lineRenderer;

    void Awake()
    {
        Instance = this;
        CreateBorderObject();
    }

    void CreateBorderObject()
    {
        borderObject = new GameObject("CombinedInfluenceBorder");
        borderObject.transform.position = Vector3.zero;

        borderObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        lineRenderer = borderObject.AddComponent<LineRenderer>();

        // Make it visible
        lineRenderer.startWidth = 0.12f;
        lineRenderer.endWidth = 0.12f;
        lineRenderer.useWorldSpace = true;

        // Use Legacy Diffuse shader
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        lineRenderer.material.color = borderColor;

        // Force colors
        lineRenderer.startColor = borderColor;
        lineRenderer.endColor = borderColor;

        // Rendering settings
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.sortingOrder = 100;
       // lineRenderer.raycastTarget = false;


        Debug.Log("LineRenderer created on: " + borderObject.name);
    }

    public void RefreshBorder()
    {
        // Get all cells influenced by any zone
        HashSet<Vector2Int> allInfluencedCells = new HashSet<Vector2Int>();
        InfluenceZone[] zones = FindObjectsByType<InfluenceZone>(FindObjectsSortMode.None);

        foreach (InfluenceZone zone in zones)
        {
            List<Vector2Int> zoneCells = zone.GetCellsInZone();
            foreach (Vector2Int cell in zoneCells)
            {
                allInfluencedCells.Add(cell);
            }
        }

        // Calculate and draw the perimeter
        if (allInfluencedCells.Count > 0)
        {
            List<Vector3> perimeterPoints = CalculatePerimeter(allInfluencedCells);
            lineRenderer.positionCount = perimeterPoints.Count;
            lineRenderer.SetPositions(perimeterPoints.ToArray());
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    List<Vector3> CalculatePerimeter(HashSet<Vector2Int> cells)
    {
        List<Edge> edges = new List<Edge>();

        // Find all outer edges
        foreach (Vector2Int cell in cells)
        {
            // Check each of 4 sides - if neighbor is not in set, it's an outer edge
            if (!cells.Contains(new Vector2Int(cell.x - 1, cell.y)))
                edges.Add(new Edge(cell.x - 0.5f, cell.y - 0.5f, cell.x - 0.5f, cell.y + 0.5f));

            if (!cells.Contains(new Vector2Int(cell.x + 1, cell.y)))
                edges.Add(new Edge(cell.x + 0.5f, cell.y - 0.5f, cell.x + 0.5f, cell.y + 0.5f));

            if (!cells.Contains(new Vector2Int(cell.x, cell.y - 1)))
                edges.Add(new Edge(cell.x - 0.5f, cell.y - 0.5f, cell.x + 0.5f, cell.y - 0.5f));

            if (!cells.Contains(new Vector2Int(cell.x, cell.y + 1)))
                edges.Add(new Edge(cell.x - 0.5f, cell.y + 0.5f, cell.x + 0.5f, cell.y + 0.5f));
        }

        // Order edges into a continuous line
        return OrderEdges(edges);
    }

    List<Vector3> OrderEdges(List<Edge> edges)
    {
        List<Vector3> points = new List<Vector3>();
        if (edges.Count == 0) return points;

        Edge current = edges[0];
        edges.RemoveAt(0);

        // Changed Y from 0.1f to 0.6f to be above tiles
        points.Add(new Vector3(current.x1, 0.6f, current.y1));
        Vector2 currentEnd = new Vector2(current.x2, current.y2);

        while (edges.Count > 0)
        {
            bool found = false;

            for (int i = 0; i < edges.Count; i++)
            {
                Edge edge = edges[i];
                Vector2 edgeStart = new Vector2(edge.x1, edge.y1);
                Vector2 edgeEnd = new Vector2(edge.x2, edge.y2);

                if (Vector2.Distance(edgeStart, currentEnd) < 0.01f)
                {
                    points.Add(new Vector3(edge.x1, 0.6f, edge.y1));
                    currentEnd = edgeEnd;
                    edges.RemoveAt(i);
                    found = true;
                    break;
                }
                else if (Vector2.Distance(edgeEnd, currentEnd) < 0.01f)
                {
                    points.Add(new Vector3(edge.x2, 0.6f, edge.y2));
                    currentEnd = edgeStart;
                    edges.RemoveAt(i);
                    found = true;
                    break;
                }
            }

            if (!found) break;
        }

        // Close the loop
        if (points.Count > 0)
            points.Add(points[0]);

        return points;
    }

    struct Edge
    {
        public float x1, y1, x2, y2;

        public Edge(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
    }

    void OnDrawGizmos()
    {
        if (lineRenderer != null && lineRenderer.positionCount > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                Vector3 start = lineRenderer.GetPosition(i);
                Vector3 end = lineRenderer.GetPosition(i + 1);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}