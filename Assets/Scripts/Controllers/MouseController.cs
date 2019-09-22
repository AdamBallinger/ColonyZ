using System;
using System.Collections.Generic;
using EzPool;
using Models.Map;
using Models.Map.Pathing;
using Models.Map.Tiles;
using Models.Sprites;
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

        private bool edgeFill;

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

            if (!isDragging && Input.GetMouseButtonDown(0) && !IsMouseOverUI)
            {
                isDragging = true;
                dragStartPosition = currentMousePosition;
            }

            if (!isDragging)
            {
                dragStartPosition = currentMousePosition;
            }

            var dragData = GetDragData();

            UpdateDragPreview(dragData);

            if (isDragging)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;

                    // Abort if dragged over a UI object.
                    if (!IsMouseOverUI)
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
            // TODO: Dragging probably needs to be rewritten as it causes massive frame drops with large drag areas.
            ClearPreviewObjects();

            // Hide selection graphic if mouse is off the map.
            selectionObject.SetActive(GetTileUnderMouse() != null);

            var selectionSize = Vector2.zero;
            var selectionPosition = Vector2.zero;

            if (Mode == MouseMode.Select)
            {
                selectionObject.SetActive(isDragging);

                if (isDragging)
                {
                    selectionSize.x = dragStartPosition.x - currentMousePosition.x;
                    selectionSize.y = dragStartPosition.y - currentMousePosition.y;

                    selectionPosition = dragStartPosition - selectionSize / 2;
                }
            }

            if (Mode == MouseMode.Build)
            {
                // Calculate size of drag area. Add one as the world starts at 0, 0
                selectionSize.x = _dragData.EndX - _dragData.StartX + 1.0f;
                selectionSize.y = _dragData.EndY - _dragData.StartY + 1.0f;

                // As pivot for the selection cursor is the center, set position based on the drag start + half the selection size.
                // minus 0.5f from the drag start X and Y so its positioned in the center of the tile (Tile are center pivoted).
                selectionPosition = new Vector2(_dragData.StartX - 0.5f, _dragData.StartY - 0.5f) + selectionSize / 2;

                edgeFill = Input.GetKey(KeyCode.LeftShift);
                
                selectionObject.SetActive(Mode == MouseMode.Select);

                for (var x = _dragData.StartX; x <= _dragData.EndX; x++)
                {
                    for (var y = _dragData.StartY; y <= _dragData.EndY; y++)
                    {
                        if(edgeFill)
                        {
                            if(x != _dragData.StartX && y != _dragData.StartY && x != _dragData.EndX && y != _dragData.EndY)
                            {
                                continue;
                            }
                        }

                        var tile = World.Instance.GetTileAt(x, y);

                        if (tile == null) continue;

                        var previewObject = previewPool.GetAvailable();
                        previewObject.transform.position = new Vector2(x, y);
                        var previewRenderer = previewObject.GetComponent<SpriteRenderer>();

                        if (BuildModeController.Mode == BuildMode.Object)
                        {
                            previewRenderer.sprite = BuildModeController.ObjectToBuild?.GetIcon();

                            // Tint the preview color based on if the structure position is valid.
                            previewRenderer.color = !World.Instance.IsObjectPositionValid(BuildModeController.ObjectToBuild, tile)
                                ? new Color(1.0f, 0.3f, 0.3f, 0.4f) : new Color(0.3f, 1.0f, 0.3f, 0.4f);
                        }

                        if (BuildModeController.Mode == BuildMode.Demolish)
                        {
                            if (tile.Object != null)
                            {
                                previewRenderer.sprite = SpriteCache.GetSprite("Overlay", 0);
                                previewRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
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
            foreach (var obj in previewObjects)
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
            if (Mode == MouseMode.Select)
            {
                return;
            }
            
            var tiles = new Tile[_dragData.SizeX, _dragData.SizeY];
            for (var x = _dragData.StartX; x <= _dragData.EndX; x++)
            {
                for (var y = _dragData.StartY; y <= _dragData.EndY; y++)
                {
                    if (edgeFill)
                    {
                        if (x != _dragData.StartX && y != _dragData.StartY && x != _dragData.EndX && y != _dragData.EndY)
                        {
                            continue;
                        }
                    }

                    var tile = World.Instance?.GetTileAt(x, y);

                    if (tile == null)
                    {
                        continue;
                    }

                    tiles[x - _dragData.StartX, y - _dragData.StartY] = tile;
                }
            }
            
            // Diagonally build over the drag area.
            for (var line = 1; line <= _dragData.SizeX + _dragData.SizeY - 1; line++)
            {
                var startCol = Math.Max(0, line - _dragData.SizeX);
                var count = Math.Min(line, Math.Min((_dragData.SizeY - startCol), _dragData.SizeX));
                for (var i = 0; i < count; i++)
                {
                    BuildModeController.Build(tiles[Math.Min(_dragData.SizeX, line) - i - 1, startCol + i]);
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
        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int EndX { get; private set; }
        public int EndY { get; private set; }

        public int SizeX => EndX - StartX + 1;
        public int SizeY => EndY - StartY + 1;

        public void Build(int _startX, int _endX, int _startY, int _endY)
        {
            StartX = Mathf.Min(_startX, _endX);
            StartY = Mathf.Min(_startY, _endY);
            EndX = Mathf.Max(_startX, _endX);
            EndY = Mathf.Max(_startY, _endY);
        }
    }
}
