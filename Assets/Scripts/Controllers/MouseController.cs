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

            //Vector2 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            //var roundedMouseWorldPos = new Vector2(Mathf.Round(mouseWorldPos.x) + 0.5f, Mathf.Round(mouseWorldPos.y) + 0.5f);

            //if(!isDragging)
            //{
            //    selectionObject.transform.position = roundedMouseWorldPos;
            //    selectionObjectRenderer.size = Vector2.one;
            //}

            //if (Input.GetMouseButtonDown(0) && !IsMouseOverUI)
            //{
            //    isDragging = true;
            //    dragStartPosition = roundedMouseWorldPos; // TODO: Later add dif mouse modes; Selection shouldn't clamp, build should.
            //    selectionObject.transform.position = dragStartPosition;
            //}

            //if (isDragging)
            //{
            //    currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            //    var selectionSize = dragStartPosition - roundedMouseWorldPos;

            //    // Create drag start and end coordinates. Add 0.5f to compensate for the fact gameobject pivots are in the center
            //    // and the tile map asumes it would be the bottom left corner of the tile.
            //    var dragStartX = (int)dragStartPosition.x;
            //    var dragEndX = (int)roundedMouseWorldPos.x;
            //    var dragStartY = (int)dragStartPosition.y;
            //    var dragEndY = (int)roundedMouseWorldPos.y;

            //    if (dragEndX < dragStartX)
            //    {
            //        if (selectionSize.x == 0)
            //        {
            //            selectionSize.x = -1;
            //        }
            //        else if(selectionSize.x > 0)
            //        {
            //            selectionSize.x = -selectionSize.x;
            //        }
                    
            //        var tmp = dragEndX;
            //        dragEndX = dragStartX;
            //        dragStartX = tmp;
            //    }
            //    else
            //    {
            //        if(selectionSize.x == 0)
            //        {
            //            selectionSize.x = 1;
            //        }
            //    }

            //    if (dragEndY < dragStartY)
            //    {
            //        if (selectionSize.y == 0)
            //        {
            //            selectionSize.y = -1;
            //        }
            //        else if(selectionSize.y > 0)
            //        {
            //            selectionSize.y = -selectionSize.y;
            //        }

            //        var tmp = dragEndY;
            //        dragEndY = dragStartY;
            //        dragStartY = tmp;
            //    }
            //    else
            //    {
            //        if(selectionSize.y == 0)
            //        {
            //            selectionSize.y = 1;
            //        }
            //    }

            //    selectionObjectRenderer.size = selectionSize;

            //    if (Input.GetMouseButtonUp(0))
            //    {
            //        isDragging = false;

            //        // Process tiles in drag area.
            //        for (var x = dragStartX; x <= dragEndX; x++)
            //        {
            //            for (var y = dragStartY; y <= dragEndY; y++)
            //            {
            //                processingTile = World.Instance.GetTileAt(x, y);

            //                if (processingTile != null)
            //                {
            //                    ProcessSelectedTile(processingTile);
            //                }
            //            }
            //        }

            //        NodeGraph.Instance.UpdateGraph(dragStartX, dragStartY, dragEndX, dragEndY);
            //    }
            //}
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
            var selectionSize = new Vector2(_dragData.StartX, _dragData.StartY) -
                                new Vector2(_dragData.EndX, _dragData.EndY) - Vector2.one;          

            selectionObjectRenderer.size = -selectionSize;

            selectionObject.transform.position = new Vector2(_dragData.StartX + 0.5f, _dragData.StartY + 0.5f)
                                                 - selectionSize / 2 - Vector2.one;
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

                    if(tile.Structure == null)
                    {
                        tile.InstallStructure(TileStructureRegistry.GetStructure("Wood_Wall"));
                    }
                    else
                    {
                        tile.UninstallStructure();
                    }
                }
            }
        }

        //private void ProcessSelectedTile(Tile _tile)
        //{
        //    if (_tile.InstalledStructure == null)
        //    {
        //        _tile.InstallStructure(TileStructureRegistry.GetStructure("Wood_Wall"));
        //    }
        //    else
        //    {
        //        _tile.UninstallStructure();
        //    }
        //}

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
        public float RealStartX { get; private set; }
        public float RealStartY { get; private set; }
        public float RealEndX { get; private set; }
        public float RealEndY { get; private set; }

        public float StartX { get; private set; }
        public float StartY { get; private set; }
        public float EndX { get; private set; }
        public float EndY { get; private set; }

        public void Build(float _startX, float _endX, float _startY, float _endY)
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
