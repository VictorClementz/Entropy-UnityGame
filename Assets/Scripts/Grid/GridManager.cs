using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] public int gridWidth = 16;
    [SerializeField] public int gridHeight = 16;
    [SerializeField] private GameObject tilePrefab; 
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial; 
    [SerializeField] private Material hoverMaterial; 

    [SerializeField] private Material[,] baseMaterials;

    [Header("Resource Materials")]
    [SerializeField] private Material corruptedMaterial;
    [SerializeField] private Material woodMaterial;
    [SerializeField] private Material stoneMaterial;
    [SerializeField] private Material ironMaterial;
    [SerializeField] private Material oilMaterial;
    [SerializeField] private Material noneMaterial;

    private GridCell2[,] grid;
    private GameObject[,] tileVisuals;
    private Camera playerCamera;

    
    private Mouse mouse;
    private Keyboard keyboard;

    // Selection state, -1 when no selection
    private Vector2Int selectedTile = new Vector2Int(-1, -1); 
    private Vector2Int hoveredTile = new Vector2Int(-1, -1);
    private void Awake()
    {
        Instance = this;
        CreateGrid();
        CreateVisuals();

    }
    void Start()
    {
        playerCamera = Camera.main;

        
        mouse = Mouse.current;
        keyboard = Keyboard.current;
       
       
    }

    void Update()
    {
        HandleMouseInput();
    }
    //Creates grid by generating gridcells
    void CreateGrid()
    {
        grid = new GridCell2[gridWidth, gridHeight];
        tileVisuals = new GameObject[gridWidth, gridHeight];
        baseMaterials = new Material[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                
                grid[x, y] = new GridCell2(x, y, false);
                grid[x, y].resourceType = GetRandomResourceType();
               

                baseMaterials[x, y] = GetMaterialForResourceType(grid[x, y].resourceType);

            }
        }

    }
    //Assigns random resources to cell, tweak ALOT later
    ResourceType GetRandomResourceType()
    {
        int random = Random.Range(0, 100);
        if (random < 30) return ResourceType.None;
        if (random < 60) return ResourceType.Wood;
        if (random < 80) return ResourceType.Stone;
        if (random < 95) return ResourceType.Iron;
        return ResourceType.Oil; // 10% chance
    }
    //Set correct material by resource type
    Material GetMaterialForResourceType(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return woodMaterial;
            case ResourceType.Stone:
                return stoneMaterial;
            case ResourceType.Iron:
                return ironMaterial;
            case ResourceType.Oil:
                return oilMaterial;
            case ResourceType.None:
                return noneMaterial;
            default:
                return noneMaterial;
        }
    }
    //Creates the visuals for the grid
    void CreateVisuals()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity);
                tile.transform.SetParent(this.transform);
                tile.name = $"Tile_{x}_{y}";

                // Add a collider for clicking if it doesn't have one
                if (tile.GetComponent<Collider>() == null)
                {
                    tile.AddComponent<BoxCollider>();
                 
                }

                var renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = baseMaterials[x, y];
                }

                tileVisuals[x, y] = tile;

               
            }
        }
    }
        /// <summary>
        /// Tile Selection
        /// </summary>
    void HandleMouseInput()
    {
        // Check if mouse is available
        if (mouse == null) return;

        // Handle hover
        Vector2Int newHoveredTile = GetTileUnderMouse();
        if (newHoveredTile != hoveredTile)
        {
            Vector2Int oldHoveredTile = hoveredTile;  
            hoveredTile = newHoveredTile;             
            UpdateHoverVisuals(oldHoveredTile);       
        }

        // Handle click
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector2Int clickedTile = GetTileUnderMouse();
            if (IsValidTile(clickedTile))
            {
                SelectTile(clickedTile);
            }
        }

        // Right click to deselect
        if (mouse.rightButton.wasPressedThisFrame)
        {
            DeselectTile();
        }
    }

    Vector2Int GetTileUnderMouse()
    {
        
        if (mouse == null) return new Vector2Int(-1, -1);

        // Get mouse position using Input System
        Vector2 mousePosition = mouse.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Convert world position back to grid coordinates
            Vector3 hitPoint = hit.transform.position;
            int x = Mathf.RoundToInt(hitPoint.x);
            int y = Mathf.RoundToInt(hitPoint.z);

            if (IsValidTile(x, y))
            {
                return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1); 
    }

    void SelectTile(Vector2Int tileCoords)
    {
        // Deselect previous tile
        if (IsValidTile(selectedTile))
        {
            
            Vector2Int oldSelectedTile = selectedTile;
            selectedTile = tileCoords;  
            UpdateTileVisual(oldSelectedTile.x, oldSelectedTile.y);
          
        }
        else
        {
            selectedTile = tileCoords;
        }


            UpdateTileVisual(selectedTile.x, selectedTile.y);

        
        Debug.Log($"Selected tile: ({selectedTile.x}, {selectedTile.y})");

        // Connect to UI
        OnTileSelected(selectedTile);
    }

    void DeselectTile()
    {
        if (IsValidTile(selectedTile))
        {
            UpdateTileVisual(selectedTile.x, selectedTile.y);
            selectedTile = new Vector2Int(-1, -1);
            Debug.Log("Tile deselected");
        }
    }

    void UpdateHoverVisuals(Vector2Int oldHoveredTile)
    {
       
        if (IsValidTile(oldHoveredTile))
        {
            UpdateTileVisual(oldHoveredTile.x, oldHoveredTile.y);
        }

        // Add hover material, will se if i can  add a glow instead
        if (IsValidTile(hoveredTile) && hoveredTile != selectedTile)
        {
            UpdateTileVisual(hoveredTile.x, hoveredTile.y);
        }
    }
   public void UpdateTileVisual(int x, int y)
    {
        if (!IsValidTile(x, y)) return;

        var renderer = tileVisuals[x, y].GetComponent<Renderer>();

        // Determine which material to use
        if (selectedTile.x == x && selectedTile.y == y)
        {
            renderer.material = selectedMaterial;
        }
        else if (hoveredTile.x == x && hoveredTile.y == y)
        {
            renderer.material = hoverMaterial;
        }
        else if (GetCell(x, y).isCorrupted)
        {
            renderer.material = corruptedMaterial;
        }
        else
        {
            // Return to the base material for this cells resource type
            renderer.material = baseMaterials[x, y];
        }
    }

    bool IsValidTile(Vector2Int coords)
    {
        return IsValidTile(coords.x, coords.y);
    }

    bool IsValidTile(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    // Acces to selected tile
    public Vector2Int GetSelectedTile()
    {
        return selectedTile;
    }

    public bool HasSelection()
    {
        return IsValidTile(selectedTile);
    }

    // Acces to grid data
    public GridCell2 GetCell(int x, int y)
    {
        if (!IsValidTile(x, y)) return null;
        return grid[x, y];
    }

    public ResourceType GetTileResource(int x, int y)
    {
        return grid[x,y].resourceType;
    }

    
    private void OnTileSelected(Vector2Int coords)
    {
        //Add ui connection
    }

    public void SetTileOccupied(int x, int y, GameObject building)
    {
        grid[x, y].isOccupied = true;
        grid[x, y].building = building;
    }
    public bool ValidPlacement(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return false;
        return !grid[x, y].isOccupied;

    }
}

