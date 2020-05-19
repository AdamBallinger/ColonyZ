using System;
using System.Collections.Generic;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Entities;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Inventory;
using ColonyZ.Models.Items;
using ColonyZ.Models.Map.Areas;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Models.Sprites;
using ColonyZ.Models.UI;
using UnityEngine;

namespace ColonyZ.Models.Map.Tiles
{
    public class Tile : ISelectable, IInventory, IEquatable<Tile>
    {
        private TileDefinition definition, oldDefinition;

        /// <summary>
        ///     Create a tile at the given x and y from a provided tile definition.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_definition"></param>
        public Tile(int _x, int _y, TileDefinition _definition)
        {
            X = _x;
            Y = _y;
            TileDefinition = _definition;
            LivingEntities = new List<LivingEntity>();
            Neighbours = new List<Tile>();
            DirectNeighbours = new List<Tile>();
        }

        public int X { get; }
        public int Y { get; }

        public Vector2 Position => new Vector2(X, Y);

        /// <summary>
        ///     List of living entities currently occupying this tile.
        /// </summary>
        public List<LivingEntity> LivingEntities { get; }

        public Job CurrentJob { get; set; }

        public Area Area { get; set; }
        public Region Region { get; set; }
        public Zone Zone { get; set; }

        /// <summary>
        ///     The definition of this tile.
        /// </summary>
        public TileDefinition TileDefinition
        {
            get => definition;
            set
            {
                oldDefinition = definition;
                definition = value;

                if (oldDefinition != definition)
                    onTileDefinitionChanged?.Invoke(this);
            }
        }

        /// <summary>
        ///     Contains all neighbours for this tile. (N, NE, E, SE, S, SW, W, NW)
        /// </summary>
        public List<Tile> Neighbours { get; }

        /// <summary>
        ///     Contains all directly connected neighbours for this tile. (N, E, S, W)
        /// </summary>
        public List<Tile> DirectNeighbours { get; }

        public bool HasObject { get; private set; }

        public bool IsMapEdge => X == 0 || X == World.Instance.Width - 1 || Y == 0 || Y == World.Instance.Height - 1;

        public TileObject Object { get; private set; }

        public ItemEntity Item { get; private set; }

        public bool Equals(Tile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public ItemStack GetItemStack()
        {
            return Item?.ItemStack;
        }

        /// <summary>
        ///     Event called when the tile has changed (Object added or removed etc.).
        /// </summary>
        public event Action<Tile> onTileChanged;

        /// <summary>
        ///     Event called when the tile definition has changed.
        /// </summary>
        public event Action<Tile> onTileDefinitionChanged;

        public void SetObject(TileObject _object, bool _checkForAreas = true)
        {
            for (var xOffset = 0; xOffset < _object.Width; xOffset++)
            for (var yOffset = 0; yOffset < _object.Height; yOffset++)
            {
                var t = World.Instance.GetTileAt(X + xOffset, Y + yOffset);

                t.Object = _object;
                t.Object.OriginTile = this;
                t.Object.Tile = t;
                t.onTileChanged?.Invoke(t);
            }

            HasObject = true;
            World.Instance.Objects.Add(_object);
            NodeGraph.Instance.UpdateGraph(_object.Tile.X, _object.Tile.Y);

            if (_checkForAreas && _object.EnclosesRoom)
                //AreaManager.Instance.CheckForArea(this);
                RegionManager.Instance.Update(this);

            onTileChanged?.Invoke(this);
        }

        public void RemoveObject(bool _checkForAreas = true)
        {
            if (!HasObject) return;

            var shouldCheckForArea = _checkForAreas && Object.EnclosesRoom;

            World.Instance.Objects.Remove(Object);
            Object = null;
            HasObject = false;
            NodeGraph.Instance.UpdateGraph(X, Y);
            onTileChanged?.Invoke(this);

            if (shouldCheckForArea)
                //AreaManager.Instance.CheckForArea(this);
                RegionManager.Instance.Update(this);
        }

        public void SetItem(ItemEntity _entity)
        {
            // Tile already has an item on it.
            if (Item != null) return;

            Item = _entity;
        }

        public void RemoveItem()
        {
            Item = null;
        }

        public TileEnterability GetEnterability()
        {
            return HasObject ? Object.Enterability : TileEnterability.Immediate;
        }

        public override string ToString()
        {
            return $"Tile: {TileDefinition.TileName}   X: {X} Y: {Y}  Obj: {(HasObject ? Object.ObjectName : "None")}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Tile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(Tile left, Tile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return !Equals(left, right);
        }

        #region Neighbour Accessbility Utility

        public Tile Left => World.Instance.GetTileAt(X - 1, Y);
        public Tile Right => World.Instance.GetTileAt(X + 1, Y);
        public Tile Up => World.Instance.GetTileAt(X, Y + 1);
        public Tile Down => World.Instance.GetTileAt(X, Y - 1);

        #endregion

        #region ISelectable Implementation

        public Sprite GetSelectionIcon()
        {
            return SpriteCache.GetSprite("Tiles", TileDefinition.TextureIndex);
        }

        public string GetSelectionName()
        {
            return TileDefinition.TileName;
        }

        public string GetSelectionDescription()
        {
            // TODO: Only include area id in selection details for development build / debug mode?
            return $"Position: ({X}, {Y})\n" +
                   $"Area ID: {(Area != null ? Area.AreaID.ToString() : "None")}\n" +
                   $"Zone: {(Zone != null ? Zone.ZoneName : "None")}\n";
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        #endregion
    }
}