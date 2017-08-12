using Models.Map;
using Models.Pathing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class MouseController : MonoBehaviour
    {

        public GameObject selectionObject;

        private SpriteRenderer selectionObjectRenderer;

        private bool isDragging;

        private bool IsMouseOverUI { get; set; }

        // World space drag start position.
        private Vector2 dragStartPosition;
        // World space position of the mouse.
        private Vector2 currentMousePosition;

        private new Camera camera;

        private Tile processingTile;

        private void Start()
        {
            camera = Camera.main;
            selectionObjectRenderer = selectionObject.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            HandleDragging();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                new PathRequest(World.Instance?.Tiles[0, 0], World.Instance?.Tiles[Random.Range(0, 144), Random.Range(0, 144)], OnPath);
            }
        }

        public LineRenderer ren;
        private void OnPath(Path _path)
        {
            if (!_path.IsValid)
            {
                Debug.Log(_path.ComputeTime < 0.0f ? "Invalid path start and/or end." : "Invalid path.");
                return;
            }

            Debug.Log($"Path generated in {_path.ComputeTime}ms.");

            ren.positionCount = _path.NodePath.Count;

            for (var i = 0; i < _path.NodePath.Count; i++)
            {
                ren.SetPosition(i, new Vector3(_path.NodePath[i].X, _path.NodePath[i].Y, 0.0f));
            }
        }

        private void HandleDragging()
        {
            IsMouseOverUI = EventSystem.current.IsPointerOverGameObject();

            if (Input.GetMouseButtonDown(0) && !IsMouseOverUI)
            {
                isDragging = true;
                dragStartPosition = camera.ScreenToWorldPoint(Input.mousePosition);
                selectionObject.SetActive(true);
                selectionObject.transform.position = dragStartPosition;
            }

            if (isDragging)
            {
                currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
                var dragDist = Vector2.Distance(currentMousePosition, dragStartPosition);

                selectionObjectRenderer.size = dragStartPosition - currentMousePosition;

                // Create drag start and end coordinates. Add 0.5f to compensate for the fact gameobject pivots are in the center
                // and the tile map asumes it would be the bottom left corner of the tile.
                var dragStartX = Mathf.FloorToInt(dragStartPosition.x + 0.5f);
                var dragEndX = Mathf.FloorToInt(currentMousePosition.x + 0.5f);
                var dragStartY = Mathf.FloorToInt(dragStartPosition.y + 0.5f);
                var dragEndY = Mathf.FloorToInt(currentMousePosition.y + 0.5f);

                if (dragEndX < dragStartX)
                {
                    var tmp = dragEndX;
                    dragEndX = dragStartX;
                    dragStartX = tmp;
                }

                if (dragEndY < dragStartY)
                {
                    var tmp = dragEndY;
                    dragEndY = dragStartY;
                    dragStartY = tmp;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                    selectionObject.SetActive(false);

                    // Experimental drag threshold as clicking and dragging feels inconsistent sometimes 
                    if (dragDist <= 0.6f)
                    {
                        dragEndX = dragStartX;
                        dragEndY = dragStartY;
                    }

                    // Process tiles in drag area.
                    for (var x = dragStartX; x <= dragEndX; x++)
                    {
                        for (var y = dragStartY; y <= dragEndY; y++)
                        {
                            processingTile = World.Instance.GetTileAt(x, y);

                            if (processingTile != null)
                            {
                                ProcessSelectedTile(processingTile);
                            }
                        }
                    }

                    //NodeGraph.Instance.BuildPartialGraph(dragStartX, dragStartY, dragEndX, dragEndY);
                    NodeGraph.Instance?.BuildFullGraph();
                }
            }
        }

        private TileSpriteData woodWallData = new TileSpriteData
        {
            IsTileSet = true,
            SpriteName = "tileset_wood_walls_",
            ResourceLocation = "Sprites/Game/Tiles/tileset_wood_walls"
        };

        //private TileSpriteData steelWallData = new TileSpriteData
        //{
        //    IsTileSet = true,
        //    SpriteName = "tileset_steel_walls_",
        //    ResourceLocation = "Sprites/Game/Tiles/tileset_steel_walls"
        //};

        private void ProcessSelectedTile(Tile _tile)
        {
            if (_tile.InstalledStructure == null)
            {
                _tile.InstallStructure(new TileStructure(1, 1, "Wood_Wall", TileStructureType.Multi_Tile, woodWallData));
            }
            //else if (_tile.InstalledStructure.StructureName.Equals("Wood_Wall"))
            //{
            //    _tile.UninstallStructure();
            //    _tile.InstallStructure(new TileStructure(1, 1, "Steel_Wall", TileStructureType.Multi_Tile, steelWallData));
            //}
            else
            {
                _tile.UninstallStructure();
            }
        }

        private Tile GetTileUnderMouse()
        {
            return World.Instance.GetTileAt(currentMousePosition);
        }
    }
}
