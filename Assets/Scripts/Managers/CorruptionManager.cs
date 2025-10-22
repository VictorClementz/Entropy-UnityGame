using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance;

    [Header("Corruption Settings")]
    [SerializeField] private float corruptionTickInterval = 3f;
    [SerializeField] private float corruptionSpeedMultiplier = 1.1f; //Speed of global spread
    [SerializeField] private float corruptionGrowthRate = 0.1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float spreadChance = 0.7f; //Start at 70% spread chance

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
            GrowCorruption();
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
            float effectiveSpreadChance = GetEffectiveSpreadChance(cell.x, cell.y); //Different spread depending on condiotn (rn only influence zone)

            if (Random.value > spreadChance) continue;

            List<(int x, int y)> neighbors = GetNeighbors(cell.x, cell.y);
            if (neighbors.Count == 0) continue;

            
            int maxSpread = neighbors.Count;
            int spreadsThisTick = Random.Range(1, maxSpread + 1); //1 to 4 neghbours affected

            //Random order
            neighbors = neighbors.OrderBy(x => Random.value).ToList();

            for (int i = 0; i < spreadsThisTick; i++)
            {
                CorruptCell(neighbors[i].x, neighbors[i].y);
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
        cell.corruptionLevel = 0.0f;
        corruptedCells.Add((x, y));

        gridManager.UpdateTileVisual(x, y);

        // will change, works for testing
        if (cell.building != null && cell.building.GetComponent<MainBuilding>() != null)
        {
            GameOver();
        }

       
    }

    void GrowCorruption()
    {
        List<(int x, int y)> cellsToGrow = new List<(int x, int y)>(corruptedCells);

        foreach (var cellPos in cellsToGrow)
        {
            GridCell2 cell = gridManager.GetCell(cellPos.x, cellPos.y);
            if (cell == null || !cell.isCorrupted) continue;

            // Increase corruption level
            cell.corruptionLevel += corruptionGrowthRate * Time.deltaTime;

            // Check thresholds for visual updates or destruction
            if (cell.corruptionLevel >= 1.0f && cell.building != null)
            {
                // Stage 3 equivalent - destroy buildings
                if (cell.building.GetComponent<Outpost>() != null)
                {
                    Debug.Log($"Outpost at ({cellPos.x}, {cellPos.y}) destroyed by corruption!");
                    Destroy(cell.building);
                    cell.building = null;
                    cell.isOccupied = false;
                }
                else if (cell.building.GetComponent<MainBuilding>() != null)
                {
                    GameOver();
                }
            }

            // Update visuals (call periodically, not every frame)
            // Could add a small timer here to reduce calls
        }
    }
    //remove corruption comp
    public void RemoveCorruptedCell(int x, int y)
    {
        corruptedCells.Remove((x, y));
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

    float GetEffectiveSpreadChance(int x, int y)
    {
        
        if (GridManager.Instance.IsPositionInAnyInfluence(x, y)) //slow spread in influece zone
        {
            return spreadChance * 0.5f; 
        }

        return spreadChance; 
    }

    //Testing
    void GameOver()
    {
        Debug.Log("GAME OVER - HQ Corrupted!");
        Time.timeScale = 0;
    }
}