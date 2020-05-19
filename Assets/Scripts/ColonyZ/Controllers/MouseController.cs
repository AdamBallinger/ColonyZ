using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Controllers.UI;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Sprites;
using EzPool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColonyZ.Controllers
{
    public enum MouseMode
    {
        Select, // Use for selecting something..
        Process, // Previews over dragged area.
        Process_Single // Previews only over mouse position. Ignores drag.
    }

    public class MouseController : MonoBehaviour
    {
        private new Camera camera;

        /// <summary>
        ///     Stores the exact world space position of the mouse.
        /// </summary>
        private Vector2 currentMousePosition;

        /// <summary>
        ///     Default color of the draggable cursor object.
        /// </summary>
        private Color defaultCursorColor;

        [SerializeField] private GameObject draggableCursor;

        private SpriteRenderer draggableCursorRenderer;

        /// <summary>
        ///     Stores the exact world space position that the mouse drag started.
        /// </summary>
        private Vector2 dragStartPosition;

        private bool isDragging;

        private Tile lastFrameTile;

        private Color previewInvalidColor = new Color(1.0f, 0.3f, 0.3f, 0.4f);

        private List<GameObject> previewObjects;
        private Color previewOverlayColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        private EzPoolManager previewPool;
        private Color previewValidColor = new Color(0.3f, 1.0f, 0.3f, 0.4f);

        private SelectionController selectionController;
        public static MouseController Instance { get; private set; }

        public BuildModeController BuildModeController { get; private set; }

        public MouseMode Mode { get; set; } = MouseMode.Select;

        private bool IsMouseOverUI { get; set; }

        /// <summary>
        ///     Event fired when the mouse is clicked. Passes tile clicked on and if the cursor was over UI.
        /// </summary>
        public event Action<Tile, bool> mouseClickEvent;

        /// <summary>
        ///     Event called when the mouse moves over a new tile. Passes new tile and if mouse is over UI.
        /// </summary>
        public event Action<Tile, bool> mouseTileChangeEvent;

        private void Awake()
        {
            Instance = this;
            BuildModeController = new BuildModeController();
        }

        private void Start()
        {
            previewPool = GetComponent<EzPoolManager>();
            selectionController = FindObjectOfType<SelectionController>();
            previewObjects = new List<GameObject>();
            camera = Camera.main;
            draggableCursorRenderer = draggableCursor.GetComponent<SpriteRenderer>();
            defaultCursorColor = draggableCursorRenderer.color;
        }

        private void Update()
        {
            HandleDragging();

            lastFrameTile = GetTileUnderMouse();
        }

        private void HandleDragging()
        {
            IsMouseOverUI = EventSystem.current.IsPointerOverGameObject();

            currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
            var currentMouseTile = GetTileUnderMouse();

            if (currentMouseTile != lastFrameTile) mouseTileChangeEvent?.Invoke(currentMouseTile, IsMouseOverUI);

            if (Input.GetMouseButtonDown(0)) mouseClickEvent?.Invoke(currentMouseTile, IsMouseOverUI);

            if (!isDragging && Input.GetMouseButtonDown(0) && !IsMouseOverUI)
            {
                isDragging = true;
                dragStartPosition = currentMousePosition;
            }

            if (!isDragging) dragStartPosition = currentMousePosition;

            var dragData = GetDragData();

            UpdateDragPreview(dragData);

            if (isDragging)
                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;

                    // Abort if dragged over a UI object.
                    if (!IsMouseOverUI) ProcessDragSelection(dragData);
                }
        }

        /// <summary>
        ///     Updates the preview for the current mouse drag.
        /// </summary>
        /// <param name="_dragData"></param>
        private void UpdateDragPreview(DragData _dragData)
        {
            ClearPreviewObjects();

            // Hide selection graphic if mouse is off the map.
            draggableCursor.SetActive(GetTileUnderMouse() != null);

            var selectionSize = Vector2.zero;
            var selectionPosition = Vector2.zero;

            if (Mode == MouseMode.Select)
            {
                draggableCursor.SetActive(isDragging);
                draggableCursorRenderer.color = defaultCursorColor;

                if (isDragging)
                {
                    selectionSize.x = dragStartPosition.x - currentMousePosition.x;
                    selectionSize.y = dragStartPosition.y - currentMousePosition.y;

                    selectionPosition = dragStartPosition - selectionSize / 2;
                }
            }
            else if (Mode == MouseMode.Process)
            {
                // Calculate size of drag area. Add one as the world starts at 0, 0
                selectionSize.x = _dragData.EndX - _dragData.StartX + 1.0f;
                selectionSize.y = _dragData.EndY - _dragData.StartY + 1.0f;

                // As pivot for the selection cursor is the center, set position based on the drag start + half the selection size.
                // minus 0.5f from the drag start X and Y so its positioned in the center of the tile (Tile are center pivoted).
                selectionPosition = new Vector2(_dragData.StartX - 0.5f, _dragData.StartY - 0.5f) + selectionSize / 2;

                draggableCursor.SetActive(false);

                var areaValid = true;

                for (var x = _dragData.StartX; x <= _dragData.EndX; x++)
                for (var y = _dragData.StartY; y <= _dragData.EndY; y++)
                {
                    if (BuildModeController.Mode == BuildMode.Object)
                        if (x != _dragData.StartX && y != _dragData.StartY && x != _dragData.EndX &&
                            y != _dragData.EndY)
                            continue;

                    // Don't allow modifying edge of map.
                    if (x == 0 || x == World.Instance.Width - 1 || y == 0 || y == World.Instance.Height - 1)
                        continue;

                    var tile = World.Instance.GetTileAt(x, y);

                    if (tile == null) continue;

                    var previewObject = previewPool.GetAvailable();
                    previewObject.transform.position = new Vector2(x, y);
                    // TODO: Should probably pool the SpriteRenderer as well as the GameObject?
                    var previewRenderer = previewObject.GetComponent<SpriteRenderer>();
                    previewRenderer.sprite = null;

                    if (BuildModeController.Mode == BuildMode.Object)
                    {
                        previewRenderer.sprite = BuildModeController.ObjectToBuild.GetIcon();

                        // Tint the preview color based on if the structure position is valid.
                        previewRenderer.color =
                            !World.Instance.IsObjectPositionValid(BuildModeController.ObjectToBuild, tile)
                                ? previewInvalidColor
                                : previewValidColor;
                    }
                    else if (BuildModeController.Mode == BuildMode.Demolish)
                    {
                        // Only allow buildable objects to be demolished.
                        if (tile.HasObject && tile.Object.Buildable)
                        {
                            previewRenderer.sprite = SpriteCache.GetSprite("Overlay", 0);
                            previewRenderer.color = previewOverlayColor;
                        }
                        else
                        {
                            previewRenderer.sprite = null;
                        }
                    }
                    else if (BuildModeController.Mode == BuildMode.Area)
                    {
                        draggableCursor.SetActive(true);
                        var area = BuildModeController.ZoneToBuild;

                        if (!area.CanPlace(tile) || _dragData.SizeX < area.MinimumSize.x ||
                            _dragData.SizeY < area.MinimumSize.y)
                        {
                            draggableCursorRenderer.color = previewInvalidColor;
                            areaValid = false;
                        }
                        else
                        {
                            if (!areaValid) break;
                            draggableCursorRenderer.color = previewValidColor;
                        }
                    }
                    else if (BuildModeController.Mode == BuildMode.Mine)
                    {
                        if (tile.HasObject && tile.Object.Mineable)
                        {
                            previewRenderer.sprite = SpriteCache.GetSprite("Overlay", 1);
                            previewRenderer.color = previewOverlayColor;
                        }
                        else
                        {
                            previewRenderer.sprite = null;
                        }
                    }
                    else if (BuildModeController.Mode == BuildMode.Fell)
                    {
                        if (tile.HasObject && tile.Object.Fellable)
                        {
                            previewRenderer.sprite = SpriteCache.GetSprite("Overlay", 2);
                            previewRenderer.color = previewOverlayColor;
                        }
                        else
                        {
                            previewRenderer.sprite = null;
                        }
                    }

                    previewObjects.Add(previewObject);
                }
            }
            else if (Mode == MouseMode.Process_Single)
            {
                var previewObject = previewPool.GetAvailable();
                var previewPos = currentMousePosition;
                previewPos.x = Mathf.FloorToInt(currentMousePosition.x + 0.5f);
                previewPos.y = Mathf.FloorToInt(currentMousePosition.y + 0.5f);
                previewObject.transform.position = previewPos;
                var previewRenderer = previewObject.GetComponent<SpriteRenderer>();
                previewRenderer.sprite = null;

                if (BuildModeController.Mode == BuildMode.Object)
                {
                    previewRenderer.sprite = BuildModeController.ObjectToBuild.GetIcon();
                    previewRenderer.color =
                        !World.Instance.IsObjectPositionValid(BuildModeController.ObjectToBuild, GetTileUnderMouse())
                            ? previewInvalidColor
                            : previewValidColor;
                }

                previewObjects.Add(previewObject);
            }

            draggableCursor.transform.position = selectionPosition;
            draggableCursorRenderer.size = selectionSize;
        }

        private void ClearPreviewObjects()
        {
            foreach (var obj in previewObjects) previewPool.PoolObject(obj);

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
                selectionController.OnTileSelect(World.Instance.GetTileAt(_dragData.StartX, _dragData.StartY));
                return;
            }

            var tiles = new Tile[_dragData.SizeX, _dragData.SizeY];
            for (var x = _dragData.StartX; x <= _dragData.EndX; x++)
            for (var y = _dragData.StartY; y <= _dragData.EndY; y++)
            {
                if (BuildModeController.Mode == BuildMode.Object)
                    // Only when building objects should the drag area only include the edge of the dragged area.
                    if (x != _dragData.StartX && y != _dragData.StartY && x != _dragData.EndX &&
                        y != _dragData.EndY)
                        continue;

                // Don't allow modifying the edge of map.
                if (x == 0 || x == World.Instance.Width - 1 || y == 0 || y == World.Instance.Height - 1) continue;

                var tile = World.Instance?.GetTileAt(x, y);

                if (tile == null) continue;

                tiles[x - _dragData.StartX, y - _dragData.StartY] = tile;
            }

            if (BuildModeController.Mode == BuildMode.Object || BuildModeController.Mode == BuildMode.Area)
            {
                BuildModeController.Process(tiles.Cast<Tile>(), _dragData.StartX, _dragData.StartY, _dragData.SizeX,
                    _dragData.SizeY);
            }
            else
            {
                var sorted = new List<Tile>();
                // Diagonally process the drag area.
                for (var line = 1; line <= _dragData.SizeX + _dragData.SizeY - 1; line++)
                {
                    var startCol = Math.Max(0, line - _dragData.SizeX);
                    var count = Math.Min(line, Math.Min(_dragData.SizeY - startCol, _dragData.SizeX));
                    for (var i = 0; i < count; i++)
                        sorted.Add(tiles[Math.Min(_dragData.SizeX, line) - i - 1, startCol + i]);
                }

                sorted.RemoveAll(t => t == null);

                BuildModeController.Process(sorted);
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