using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Utils;

namespace ColonyZ.Models.Map.Areas
{
    public class AreaManager
    {
        public static AreaManager Instance { get; private set; }

        public List<Area> Areas { get; }

        /// <summary>
        ///     Event called after new area have been created, or old areas deleted.
        /// </summary>
        public event Action areasUpdatedEvent;

        private bool shouldTriggerUpdate;

        private AreaManager()
        {
            Areas = new List<Area>();
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new AreaManager();
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public int GetAreaID(Area _area)
        {
            return Areas.IndexOf(_area) + 1;
        }

        private void RemoveArea(Area _area)
        {
            if (_area == null) return;

            _area.ReleaseTiles();
            Areas.Remove(_area);
            shouldTriggerUpdate = true;
        }

        public void CheckForArea(Tile _tile)
        {
            if (_tile.IsMapEdge) return;

            var oldArea = _tile.Area;

            Predicate<Tile> floodfill_ConditionCheck = t => t != null
                                                            && t.Area == oldArea
                                                            && !(t.HasObject && t.Object.EnclosesRoom);

            Predicate<Tile> floodfill_PassCheck = t => t.Area == oldArea;

            // An enclosing object was built on this tile.
            if (oldArea != null)
            {
                var enclosingCount = 0;
                foreach (var tile in _tile.Neighbours)
                {
                    if (tile != null && tile.HasObject && tile.Object.EnclosesRoom) enclosingCount++;
                    if (enclosingCount >= 2) break;
                }

                // When an object is placed, we can assume unless it has at least 2 other enclosing objects around it,
                // then it is not enclosing a new area, and can simply remove the tile from its area.
                if (enclosingCount >= 2)
                {
                    // Flood each neighbour to see if any are now enclosed.
                    foreach (var tile in _tile.DirectNeighbours)
                        FloodFiller.Flood(tile,
                            floodfill_ConditionCheck,
                            floodfill_PassCheck,
                            CreateArea);

                    // Remove the source tile from its current area, as enclosing tiles do not belong
                    // to any areas.
                    oldArea.UnassignTile(_tile);

                    // Delete the old source tile area as it is no longer needed.
                    RemoveArea(oldArea);
                }
                else
                {
                    // Tile doesn't have at least 2 enclosing tiles around it, so just remove it from the area as
                    // it can't be enclosing an area.
                    oldArea.UnassignTile(_tile);

                    // Make sure the area is removed if it no longer has any tiles.
                    if (oldArea.Size == 0) RemoveArea(oldArea);
                }
            }
            else
            {
                // Getting here means the tile previously had an enclosing object (Wall, door etc.) on it
                // So go through each of the neighbour tiles and remove their area, as it means we could
                // potentially be merging up to 4 areas together.
                //foreach (var tile in _tile.DirectNeighbours) RemoveArea(tile.Area);

                var largestAreaSize = 0;
                Area largestArea = null;
                foreach (var tile in _tile.DirectNeighbours)
                {
                    if (tile.Area != null)
                    {
                        if (tile.Area.Size > largestAreaSize)
                        {
                            largestAreaSize = tile.Area.Size;
                            largestArea = tile.Area;
                        }
                    }
                }

                // If a larger area exists around the tile removed, then merge all surrounding areas into it.
                if (largestArea != null)
                {
                    largestArea.AssignTile(_tile);
                    foreach (var tile in _tile.DirectNeighbours)
                    {
                        if (tile.Area != null && tile.Area != largestArea)
                        {
                            MergeAreas(tile.Area, largestArea);
                        }
                    }

                    shouldTriggerUpdate = true;
                }
                else // No area was detected around the removed tile, so create a new one.
                {
                    FloodFiller.Flood(_tile,
                        floodfill_ConditionCheck,
                        floodfill_PassCheck,
                        CreateArea);
                }
            }

            if (shouldTriggerUpdate)
            {
                areasUpdatedEvent?.Invoke();
                shouldTriggerUpdate = false;
            }

            ComputeAreaLinks();
        }

        private void CreateArea(List<Tile> _tiles)
        {
            if (_tiles != null && _tiles.Count > 0)
            {
                var area = new Area();
                _tiles.ForEach(t => area.AssignTile(t));
                Areas.Add(area);
                shouldTriggerUpdate = true;
            }
        }

        /// <summary>
        ///     Merges target area into the source area.
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="_source"></param>
        private void MergeAreas(Area _target, Area _source)
        {
            _target.ReleaseTo(_source);
            RemoveArea(_target);
            shouldTriggerUpdate = true;
        }

        private void ComputeAreaLinks()
        {
            var doors = World.Instance.Objects.OfType<DoorObject>().ToList();

            // Clear existing connections, then re-add self connections.
            foreach (var area in Areas)
            {
                area.LinkedAreas.Clear();
                area.AddConnection(area);
            }

            foreach (var door in doors)
            {
                var tile = door.Tile;
                var n = World.Instance.GetTileAt(tile.X, tile.Y + 1);
                var s = World.Instance.GetTileAt(tile.X, tile.Y - 1);

                // if the tiles to the north and south of the door have different area ids,
                // then they are connected.
                if (n != null && s != null && n.Area?.AreaID != s.Area?.AreaID)
                {
                    n.Area?.AddConnection(s.Area);
                    s.Area?.AddConnection(n.Area);
                    continue;
                }

                var e = World.Instance.GetTileAt(tile.X + 1, tile.Y);
                var w = World.Instance.GetTileAt(tile.X - 1, tile.Y);

                if (e != null && w != null && e.Area?.AreaID != w.Area?.AreaID)
                {
                    e.Area?.AddConnection(w.Area);
                    w.Area?.AddConnection(e.Area);
                }
            }

            foreach (var area in Areas)
            {
                var checkedAreas = new List<Area>();
                var areas = LinkArea(area, checkedAreas);

                foreach (var r in areas) area.AddConnection(r);
            }
        }

        private List<Area> LinkArea(Area _area, List<Area> _checkedAreas)
        {
            foreach (var area in _area.LinkedAreas)
            {
                if (_checkedAreas.Contains(area)) continue;

                _checkedAreas.Add(area);
                LinkArea(area, _checkedAreas);
            }

            return _checkedAreas;
        }
    }
}