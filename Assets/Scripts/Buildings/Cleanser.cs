
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cleanser : Building
{
    [Header("Cleansing Settings")]
    [SerializeField] private float cleanseRate = 0.2f; 
    [SerializeField] private CleanseMode currentMode = CleanseMode.Focused;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;

    private Vector2Int currentPosition;
    private Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();
    private bool isMoving = false;
    //private GridManager gridManager;
    
    private bool isActive = true;

    public enum CleanseMode
    {
        Focused,  
        Area     
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Cleanser Awake called");
        gridManager = GridManager.Instance;
    }

    void Start()
    {
        


    }

    void Update()
    {
        base.Update();

        currentPosition = new Vector2Int(
           Mathf.RoundToInt(transform.position.x),
           Mathf.RoundToInt(transform.position.z)
       );
        if (!isActive) return;

        if (isMoving)
        {
            HandleMovement();
        }
        else // Only cleanse when not moving
        {
            if (currentMode == CleanseMode.Focused)
            {
                CleanseFocused();
            }
            else if (currentMode == CleanseMode.Area)
            {
                CleanseArea();
            }
        }
    }

    void HandleMovement()
    {
        if (pathQueue.Count == 0)
        {
            isMoving = false;
            return;
        }

        Vector2Int nextPos = pathQueue.Peek();
        Vector3 targetWorldPos = new Vector3(nextPos.x, transform.position.y, nextPos.y);

        //Move towards the next waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPos,
            moveSpeed * Time.deltaTime
        );

        //Check if reached the waypoint
        if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        {
            transform.position = targetWorldPos;
            currentPosition = pathQueue.Dequeue();

            if (pathQueue.Count == 0)
            {
                Debug.Log($"Cleanser arrived at ({currentPosition.x}, {currentPosition.y})");
            }
        }
    }

    void CleanseFocused()
    {
        GridCell2 cell = gridManager.GetCell(currentPosition.x, currentPosition.y);
        
        if (cell != null && cell.isCorrupted)
        {
            CleanseCell(cell);
            Debug.Log(cell.corruptionLevel);
        }
    }

    void CleanseArea()  
    {
        //Cleanse 3x3 area around cleanser
        for (int x = currentPosition.x - 1; x <= currentPosition.x + 1; x++)
        {
            for (int y = currentPosition.y - 1; y <= currentPosition.y + 1; y++)
            {
                GridCell2 cell = gridManager.GetCell(x, y);

                if (cell != null && cell.isCorrupted)
                {
                    //Cleanse time reduction
                    CleanseCell(cell, cleanseRate * 0.3f);
                    
                }
            }
        }
    }

    void CleanseCell(GridCell2 cell, float rate = -1)
    {
        if (rate < 0) rate = cleanseRate;

        //Reduce corruption level
        cell.corruptionLevel -= rate * Time.deltaTime;
        Debug.Log("Cleansing" + cell);
        if (cell.corruptionLevel <= 0f)
        {
            cell.corruptionLevel = 0f;
            cell.isCorrupted = false;
           

            CorruptionManager.Instance.RemoveCorruptedCell(cell.x, cell.y);
        }

        gridManager.UpdateTileVisual(cell.x, cell.y);
    }

    //Movement
    public void SetDestination(int x, int y)
    {
        if (!GridManager.Instance.IsValidTile(x, y) || !GridManager.Instance.ValidPlacement(x, y))
        {
            Debug.LogWarning("Invalid destination");
            return;
        }

        List<Vector2Int> path = FindPath(currentPosition, new Vector2Int(x, y));

        if (path == null)
        {
            Debug.LogWarning("No path found");
            return;
        }

        pathQueue = new Queue<Vector2Int>(path);
        isMoving = true;
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == end)
                return ReconstructPath(cameFrom, start, end);

            foreach (Vector2Int next in GetNeighbors(current, end))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        return null;
    }

    List<Vector2Int> GetNeighbors(Vector2Int pos, Vector2Int destination)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int next = pos + dir;
            if (GridManager.Instance.IsValidTile(next.x, next.y) &&
                (next == destination || GridManager.Instance.ValidPlacement(next.x, next.y)))
            {
                neighbors.Add(next);
            }
        }

        return neighbors;
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    
   
    public bool IsMoving() => isMoving;
    public Vector2Int GetCurrentPosition() => currentPosition;

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public void SetMode(CleanseMode mode)
    {
        currentMode = mode;
       
    }

    public CleanseMode GetMode()
    {
        return currentMode;
    }
}