using Models.Map;
using Models.Pathing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class MouseController : MonoBehaviour
    {
        public static MouseController Instance { get; private set; }

        public BuildModeController BuildModeController { get; private set; }

        public GameObject selectionObject;

        private SpriteRenderer selectionObjectRenderer;

        private bool isDragging;

        private bool IsMouseOverUI { get; set; }

        // World space drag start position.
        private Vector2 dragStartPosition;
        // World space position of the mouse.
        private Vector2 currentMousePosition;

        private new Camera camera;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            camera = Camera.main;
            selectionObjectRenderer = selectionObject.GetComponent<SpriteRenderer>();

            BuildModeController = new BuildModeController
            {
                Mode = BuildMode.Structure,
                StructureName = "Wood_Wall"
            };
        }

        private void Update()
        {
            HandleDragging();

            if(Input.GetKeyDown(KeyCode.B))
            {
                BuildModeController.Mode = BuildMode.Structure;
            }

            if(Input.GetKeyDown(KeyCode.F))
            {
                BuildModeController.Mode = BuildMode.Demolish;
            }
        }

        private void HandleDragging()
        {
            IsMouseOverUI = EventSystem.current.IsPointerOverGameObject();

            currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            if(!isDragging && Input.GetMouseButtonDown(0) && !IsMouseOverUI)
            {
                isDragging = true;
                dragStartPosition = currentMousePosition;
            }

            if(!isDragging)
            {
                dragStartPosition = currentMousePosition;
            }

            var dragData = GetDragData();

            UpdateDragPreview(dragData);

            if(isDragging)
            {
                if(Input.GetMouseButtonUp(0))
                {
                    isDragging = false;

                    // Abort if dragged over a UI object.
                    if(!IsMouseOverUI)
                    {
                        ProcessDragSelection(dragData);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the preview for the current mouse drag.
        /// </summary>
        /// <param name="_dragData"></param>
        private void UpdateDragPreview(DragData _dragData)
        {
            // TODO: With multiple mouse modes (Select, Build, etc.) change what the preview is based on that.
            // Hide cursor if off the map.
            selectionObject.SetActive(GetTileUnderMouse() != null);

            // Calculates the size of the cursor based on the drag distance.
            var selectionSize = new Vector2(_dragData.EndX, _dragData.EndY) -
                                new Vector2(_dragData.StartX, _dragData.StartY) + Vector2.one; // Add one so its not 0 for single tile selection       

            selectionObjectRenderer.size = selectionSize;

            // As pivot for the selection cursor is the center, set position based on the drag start + half the selection size.
            // minus 0.5f from the drag start X and Y so its positioned in the center of the tile (Tile are center pivoted).
            selectionObject.transform.position = new Vector2(_dragData.StartX - 0.5f, _dragData.StartY - 0.5f) + selectionSize / 2;
        }

        private DragData GetDragData()
        {
            var dragData = new DragData();
            dragData.Build(Mathf.FloorToInt(dragStartPosition.x + 0.5f),
                            Mathf.FloorToInt(currentMousePosition.x + 0.5f),
                            Mathf.FloorToInt(dragStartPosition.y + 0.5f),
                            Mathf.FloorToInt(currentMousePosition.y + 0.5f));

            return dragData;
        }

        private void ProcessDragSelection(DragData _dragData)
        {
            for(var x = _dragData.StartX; x <= _dragData.EndX; x++)
            {
                for(var y = _dragData.StartY; y <= _dragData.EndY; y++)
                {
                    var tile = World.Instance?.GetTileAt(x, y);

                    if(tile == null)
                    {
                        continue;
                    }

                    BuildModeController.Build(tile);
                }
            }

            NodeGraph.Instance?.UpdateGraph(_dragData.StartX, _dragData.StartY, _dragData.EndX, _dragData.EndY);
        }

        private Tile GetTileUnderMouse()
        {
            var pos = currentMousePosition;
            pos.x += 0.5f;
            pos.y += 0.5f;
            return World.Instance.GetTileAt(pos);
        }
    }

    public struct DragData
    {
        public int RealStartX { get; private set; }
        public int RealStartY { get; private set; }
        public int RealEndX { get; private set; }
        public int RealEndY { get; private set; }

        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int EndX { get; private set; }
        public int EndY { get; private set; }

        public void Build(int _startX, int _endX, int _startY, int _endY)
        {
            RealStartX = _startX;
            RealStartY = _startY;
            RealEndX = _endX;
            RealEndY = _endY;

            StartX = Mathf.Min(_startX, _endX);
            StartY = Mathf.Min(_startY, _endY);
            EndX = Mathf.Max(_startX, _endX);
            EndY = Mathf.Max(_startY, _endY);
        }
    }
}
