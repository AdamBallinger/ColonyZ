using System.Collections.Generic;
using EzPool;
using Models.Map;
using Models.Map.Structures;
using Models.Pathing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers
{
    public enum MouseMode
    {
        Select,
        Build
    }

    public class MouseController : MonoBehaviour
    {
        public static MouseController Instance { get; private set; }

        public BuildModeController BuildModeController { get; private set; }

        public MouseMode Mode { get; set; } = MouseMode.Select;

        public GameObject selectionObject;

        private SpriteRenderer selectionObjectRenderer;

        private bool isDragging;

        private bool IsMouseOverUI { get; set; }

        private EzPoolManager previewPool;

        private List<GameObject> previewObjects;

        /// <summary>
        /// Stores the exact world space position that the mouse drag started.
        /// </summary>
        private Vector2 dragStartPosition;

        /// <summary>
        /// Stores the exact world space position of the mouse.
        /// </summary>
        private Vector2 currentMousePosition;

        private new Camera camera;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            previewPool = GetComponent<EzPoolManager>();
            previewObjects = new List<GameObject>();
            camera = Camera.main;
            selectionObjectRenderer = selectionObject.GetComponent<SpriteRenderer>();

            BuildModeController = new BuildModeController();
        }

        private void Update()
        {
            HandleDragging();
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
            ClearPreviewObjects();

            // Hide selection graphic if mouse is off the map.
            selectionObject.SetActive(GetTileUnderMouse() != null);

            var selectionSize = Vector2.zero;
            var selectionPosition = Vector2.zero;

            if(Mode == MouseMode.Select)
            {
                selectionObject.SetActive(isDragging);
                
                if(isDragging)
                {
                    selectionSize.x = dragStartPosition.x - currentMousePosition.x;
                    selectionSize.y = dragStartPosition.y - currentMousePosition.y;

                    selectionPosition = dragStartPosition - selectionSize / 2;
                }
            }

            if(Mode == MouseMode.Build)
            {
                // Calculate size of drag area. Add one as the world starts at 0, 0
                selectionSize.x = _dragData.EndX - _dragData.StartX + 1.0f;
                selectionSize.y = _dragData.EndY - _dragData.StartY + 1.0f;

                // As pivot for the selection cursor is the center, set position based on the drag start + half the selection size.
                // minus 0.5f from the drag start X and Y so its positioned in the center of the tile (Tile are center pivoted).
                selectionPosition = new Vector2(_dragData.StartX - 0.5f, _dragData.StartY - 0.5f) + selectionSize / 2;

                for(var x = _dragData.StartX; x <= _dragData.EndX; x++)
                {
                    for(var y = _dragData.StartY; y <= _dragData.EndY; y++)
                    {
                        var tile = World.Instance.GetTileAt(x, y);
                        
                        if(tile == null) continue;

                        var previewObject = previewPool.GetAvailable();
                        previewObject.transform.position = new Vector2(x, y);
                        var previewRenderer = previewObject.GetComponent<SpriteRenderer>();

                        if (BuildModeController.Mode == BuildMode.Structure)
                        {
                            previewRenderer.sprite = BuildModeController.Structure?.GetIcon();

                            // Tint the preview color based on if the structure position is valid.
                            previewRenderer.color = !World.Instance.IsStructurePositionValid(BuildModeController.Structure, tile) 
                                ? new Color(1.0f, 0.3f, 0.3f, 0.6f) : new Color(0.3f, 1.0f, 0.3f, 0.6f);
                        }

                        if(BuildModeController.Mode == BuildMode.Demolish)
                        {
                            if(tile.Structure != null)
                            {
                                previewRenderer.sprite = SpriteCache.GetSprite("Overlay", "demolish");
                                previewRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
                            }
                            else
                            {
                                previewRenderer.sprite = null;
                            }
                        }

                        previewObjects.Add(previewObject);
                    }
                }             
            }

            selectionObject.transform.position = selectionPosition;
            selectionObjectRenderer.size = selectionSize;
        }

        private void ClearPreviewObjects()
        {
            foreach(var obj in previewObjects)
            {
                previewPool.PoolObject(obj);           
            }

            previewObjects.Clear();
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
            if(Mode == MouseMode.Select)
            {
                return;
            }

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
