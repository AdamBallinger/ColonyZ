using System;
using System.Collections.Generic;
using System.Linq;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using Utils;

namespace Models.Map.Areas
{
    public class AreaManager
    {
        public static AreaManager Instance { get; private set; }

        public List<Area> Areas { get; }

        /// <summary>
        /// Event called after new area have been created, or old areas deleted.
        /// </summary>
        public event Action areasUpdatedEvent;

        private bool shouldTriggerUpdate = false;

        private AreaManager()
        {
            Areas = new List<Area>();
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new AreaManager();
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
                // TODO: Try optimise this so it doesnt release all tiles in the area it was placed.
                // Causes massive slow downs on large maps. This is a downside to not having an
                // outside area system.. but its going to make stuff so much easier if this works..
                // Flood each neighbour to see if any are now enclosed.
                foreach (var tile in _tile.DirectNeighbours)
                {
                    FloodFiller.Flood(tile,
                        floodfill_ConditionCheck,
                        floodfill_PassCheck,
                        set => CreateArea(set.ToList()));
                }

                // Remove the source tile from its current area, as enclosing tiles do not belong
                // to any areas.
                oldArea.UnassignTile(_tile);

                // Delete the old source tile area as it is no longer needed.
                RemoveArea(oldArea);
            }
            else
            {
                // Getting here means the tile previously had an enclosing object (Wall, door etc.) on it
                // So go through each of the neighbour tiles and remove their area, as it means we could
                // potentially be merging up to 4 areas together.
                foreach (var tile in _tile.DirectNeighbours)
                {
                    RemoveArea(tile.Area);
                }

                // Old area removed, flood from source to find new areas.
                FloodFiller.Flood(_tile,
                    floodfill_ConditionCheck,
                    floodfill_PassCheck,
                    set => CreateArea(set.ToList()));
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

                foreach (var r in areas)
                {
                    area.AddConnection(r);
                }
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