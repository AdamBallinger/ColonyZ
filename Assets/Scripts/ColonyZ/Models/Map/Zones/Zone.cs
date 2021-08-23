using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Saving;
using ColonyZ.Models.UI;
using ColonyZ.Models.UI.Context;
using ColonyZ.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.Map.Zones
{
    public abstract class Zone : ISaveable, ISelectable, IContextProvider
    {
        public string ZoneName { get; protected set; }

        /// <summary>
        ///     Determines if the zone can contain objects when being placed in the word.
        ///     This does not affect the zone when an object is being built after the zone is placed.
        /// </summary>
        public bool CanContainObjects { get; protected set; }

        /// <summary>
        ///     Number of tiles assigned to this zone.
        /// </summary>
        public int Size => Tiles.Count;

        public List<Tile> Tiles { get; }
        
        public Color Color { get; protected set; }

        protected Zone()
        {
            Tiles = new List<Tile>();
        }

        public void AddTile(Tile _tile)
        {
            if (Tiles.Contains(_tile)) return;
            
            Tiles.Add(_tile);
            _tile.Zone = this;
            
            //ZoneManager.Instance.OnZonePlaced(this);
        }

        public void AddTiles(IEnumerable<Tile> _tiles, bool _updateManager = true)
        {
            foreach (var tile in _tiles) AddTile(tile);
            
            if (_updateManager)
                ZoneManager.Instance.OnZonePlaced(this);
        }

        public void RemoveTile(Tile _tile)
        {
            if (!Tiles.Contains(_tile)) return;

            Tiles.Remove(_tile);
            _tile.Zone = null;

            var tileLists = new List<List<Tile>>();

            foreach (var tile in _tile.DirectNeighbours)
            {
                if (tile.Zone == null) continue;

                FloodFiller.Flood(tile,
                    t => Tiles.Contains(t),
                    t => t.Zone == ZoneManager.Instance.CurrentZoneBeingModified,
                    list =>
                    {
                        if (tileLists.Any(l => l.Intersect(list).Any()))
                        {
                            return;
                        }
                        
                        tileLists.Add(list);
                    });
            }

            if (tileLists.Count > 1)
            {
                var listToKeep = tileLists[0];

                foreach (var list in tileLists)
                {
                    if (list.Count > listToKeep.Count)
                    {
                        listToKeep = list;
                    }
                }
                
                tileLists.Remove(listToKeep);
            
                foreach (var list in tileLists)
                {
                    foreach (var tile in list)
                    {
                        tile.Zone = null;
                        Tiles.Remove(tile);
                    }
                }
            }

            ZoneManager.Instance.OnZoneRemoved(this);
        }

        public void RemoveAllTiles()
        {
            foreach (var tile in Tiles)
            {
                tile.Zone = null;
            }

            Tiles.Clear();
        }

        public virtual bool CanPlace(Tile _tile)
        {
            if (_tile.Zone != null) return false;
            if (_tile.HasObject && !CanContainObjects) return false;

            return true;
        }

        public bool CanSave()
        {
            return true;
        }

        public virtual void OnSave(SaveGameWriter _writer)
        {
            _writer.WriteProperty("type", GetType().FullName);
            _writer.WriteSet("tiles", GetTileIndexes());
        }

        public virtual void OnLoad(JToken _dataToken)
        {
            var indexes = _dataToken["tiles"].ToObject<int[]>();

            foreach (var tileIndex in indexes)
            {
                AddTile(World.Instance.GetTileAt(tileIndex));
            }
            
            ZoneManager.Instance.OnZonePlaced(this);
        }

        private int[] GetTileIndexes()
        {
            var result = new int[Tiles.Count];
            for (var i = 0; i < Tiles.Count; i++)
            {
                result[i] = World.Instance.GetTileIndex(Tiles[i]);
            }

            return result;
        }

        public abstract string GetSelectionName();

        public abstract string GetSelectionDescription();

        public Vector2 GetPosition()
        {
            return Vector2.zero;
        }

        public ContextAction[] GetContextActions()
        {
            return new[]
            {
                new ContextAction("Delete", () => ZoneManager.Instance.RemoveZone(this)),
                new ContextAction("Expand", () => World.Instance.WorldActionProcessor.SetZoneExpandMode()),
                new ContextAction("Shrink", () => World.Instance.WorldActionProcessor.SetZoneShrinkMode())
            };
        }

        public string GetContextMenuName()
        {
            return ZoneName;
        }
    }
}