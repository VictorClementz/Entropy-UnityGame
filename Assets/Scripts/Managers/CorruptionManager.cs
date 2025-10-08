using UnityEngine;
using System.Collections.Generic;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance;

    [Header("Corruption Settings")]
    [SerializeField] private float corruptionTickInterval = 3f;
    [SerializeField] private float corruptionSpeedMultiplier = 1.1f;

    private GridManager gridManager;
    private float tickTimer;
    private float gameTime;

    private List<(int x, int y)> corruptedCells;

    void Awake()
    {
        
    }

    void Start()
    {
        Instance = this;
        corruptedCells = new List<(int x, int y)>();
        gridManager = GridManager.Instance;

        SpawnInitialCorruption();
    }
    //will add time globally sooner then later but works now
    void Update()
    {
        gameTime += Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= GetCurrentTickInterval())
        {
            tickTimer = 0f;
            SpreadCorruption();
        }
    }
    //ramps upp corruption
    float GetCurrentTickInterval()
    {
        float speedIncrease = Mathf.Pow(corruptionSpeedMultiplier, gameTime / 60f);
        return corruptionTickInterval / speedIncrease;
    }

    void SpawnInitialCorruption()
    {
        int gridWidth = gridManager.gridWidth;
        int gridHeight = gridManager.gridHeight;

        List<(int x, int y)> edgeCells = new List<(int x, int y)>();

        //Top and bottom edges
        for (int x = 0; x < gridWidth; x++)
        {
            edgeCells.Add((x, 0));
            edgeCells.Add((x, gridHeight - 1));
        }

        //Left and right edges
        for (int y = 1; y < gridHeight - 1; y++)
        {
            edgeCells.Add((0, y));
            edgeCells.Add((gridWidth - 1, y));
        }

        var startCell = edgeCells[Random.Range(0, edgeCells.Count)];
        CorruptCell(startCell.x, startCell.y);
    }

    void SpreadCorruption()
    {
        //Precaution
        List<(int x, int y)> cellsToSpreadFrom = new List<(int x, int y)>(corruptedCells);

        foreach (var cell in cellsToSpreadFrom)
        {
            List<(int x, int y)> neighbors = GetNeighbors(cell.x, cell.y);

            if (neighbors.Count > 0)
            {
                var targetCell = neighbors[Random.Range(0, neighbors.Count)];
                CorruptCell(targetCell.x, targetCell.y);
            }
        }
    }

    void CorruptCell(int x, int y)
    {
        GridCell2 cell = gridManager.GetCell(x, y);

        if (cell == null || cell.isCorrupted)
        {
            return;
        }
            
        //corrupt cell and add to list
        cell.isCorrupted = true;
        cell.corruptionLevel = 1f;
        corruptedCells.Add((x, y));

        gridManager.UpdateTileVisual(x, y);

        // will change, works for testing
        if (cell.building != null && cell.building.GetComponent<MainBuilding>() != null)
        {
            GameOver();
        }

        Debug.Log($"Cell ({x},{y}) corrupted!");
    }

    List<(int x, int y)> GetNeighbors(int x, int y)
    {
        List<(int x, int y)> neighbors = new List<(int x, int y)>();

        // Check all 4 directions
        (int x, int y)[] directions = new (int x, int y)[]
        {
            (x + 1, y),
            (x - 1, y),
            (x, y + 1),
            (x, y - 1)
        };

        foreach (var dir in directions)
        {
            GridCell2 cell = gridManager.GetCell(dir.x, dir.y);

            if (cell != null && !cell.isCorrupted)
            {
                neighbors.Add(dir);
            }
        }

        return neighbors;
    }
    //Testing
    void GameOver()
    {
        Debug.Log("GAME OVER - HQ Corrupted!");
        Time.timeScale = 0;
    }
}