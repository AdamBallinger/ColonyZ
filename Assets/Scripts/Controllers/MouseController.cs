using Models.World;
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
        }

        private void HandleDragging()
        {
            IsMouseOverUI = EventSystem.current.IsPointerOverGameObject();

            if(Input.GetMouseButtonDown(0) && !IsMouseOverUI)
            {
                isDragging = true;
                dragStartPosition = camera.ScreenToWorldPoint(Input.mousePosition);
                selectionObject.SetActive(true);
                selectionObject.transform.position = dragStartPosition;
            }

            if(isDragging)
            {
                currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
                selectionObjectRenderer.size = dragStartPosition - currentMousePosition;

                // Create drag start and end coordinates. Add 0.5f to compensate for the fact gameobject pivots are in the center
                // and the tile map asumes it would be the bottom left corner of the tile.
                var dragStartX = Mathf.FloorToInt(dragStartPosition.x + 0.5f);
                var dragEndX = Mathf.FloorToInt(currentMousePosition.x + 0.5f);
                var dragStartY = Mathf.FloorToInt(dragStartPosition.y + 0.5f);
                var dragEndY = Mathf.FloorToInt(currentMousePosition.y + 0.5f);

                if(dragEndX < dragStartX)
                {
                    var tmp = dragEndX;
                    dragEndX = dragStartX;
                    dragStartX = tmp;
                }

                if(dragEndY < dragStartY)
                {
                    var tmp = dragEndY;
                    dragEndY = dragStartY;
                    dragStartY = tmp;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                    selectionObject.SetActive(false);
                    
                    for(var x = dragStartX; x <= dragEndX; x++)
                    {
                        for(var y = dragStartY; y <= dragEndY; y++)
                        {
                            processingTile = World.Instance.GetTileAt(x, y);
                            
                            if(processingTile != null)
                            {
                                ProcessSelectedTile(processingTile);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessSelectedTile(Tile _tile)
        {
            if(_tile.InstalledStructure == null)
            {
                _tile.InstallStructure(new TileStructure(1, 1, TileStructureType.Wall, new TileSpriteData
                {
                    IsTileSet = true,
                    SpriteName = "tileset_walls_",
                    SpriteResourceLocation = "Sprites/Game/Tiles/tileset_wood_walls"
                }));
            }
            else
            {
                _tile.UninstallStructure();
            }
        }
	}
}
