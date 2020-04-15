using System;
using System.Collections.Generic;
using Models.AI.Jobs;
using Models.Entities;
using Models.Entities.Living;
using Models.Inventory;
using Models.Items;
using Models.Map.Pathing;
using Models.Map.Regions;
using Models.Map.Rooms;
using Models.Map.Tiles.Objects;
using Models.Map.Zones;
using Models.Sprites;
using Models.UI;
using UnityEngine;

namespace Models.Map.Tiles
{
    public class Tile : ISelectable, IInventory, IEquatable<Tile>
    {
        public int X { get; }
        public int Y { get; }

        public Vector2 Position => new Vector2(X, Y);

        /// <summary>
        /// List of living entities currently occupying this tile.
        /// </summary>
        public List<LivingEntity> LivingEntities { get; private set; }

        public Job CurrentJob { get; set; }

        public Room Room { get; set; }
        public Region Region { get; set; }
        public Zone Zone { get; set; }

        /// <summary>
        /// The definition of this tile.
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
        /// Event called when the tile has changed (Object added or removed etc.).
        /// </summary>
        public event Action<Tile> onTileChanged;

        /// <summary>
        /// Event called when the tile definition has changed.
        /// </summary>
        public event Action<Tile> onTileDefinitionChanged;

        /// <summary>
        /// Contains all neighbours for this tile. (N, NE, E, SE, S, SW, W, NW)
        /// </summary>
        public List<Tile> Neighbours { get; }

        /// <summary>
        /// Contains all directly connected neighbours for this tile. (N, E, S, W)
        /// </summary>
        public List<Tile> DirectNeighbours { get; }

        public bool HasObject { get; private set; }

        public bool IsMapEdge => X == 0 || X == World.Instance.Width - 1 || Y == 0 || Y == World.Instance.Height - 1;

        public TileObject Object { get; private set; }

        public ItemEntity Item { get; private set; }

        private TileDefinition definition, oldDefinition;

        /// <summary>
        /// Create a tile at the given x and y from a provided tile definition.
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

        public void SetObject(TileObject _object, bool _checkForRoom = true)
        {
            for (var xOffset = 0; xOffset < _object.Width; xOffset++)
            {
                for (var yOffset = 0; yOffset < _object.Height; yOffset++)
                {
                    var t = World.Instance.GetTileAt(X + xOffset, Y + yOffset);

                    t.Object = _object;
                    t.Object.OriginTile = this;
                    t.Object.Tile = t;
                    t.onTileChanged?.Invoke(t);
                }
            }

            HasObject = true;
            World.Instance.Objects.Add(_object);
            NodeGraph.Instance.UpdateGraph(_object.Tile.X, _object.Tile.Y);

            if (_checkForRoom && _object.EnclosesRoom)
            {
                RoomManager.Instance.CheckForRoom(this);
            }

            onTileChanged?.Invoke(this);
        }

        public void RemoveObject(bool _checkForRooms = true)
        {
            if (!HasObject)
            {
                return;
            }

            var shouldCheckForRoom = _checkForRooms && Object.EnclosesRoom;

            World.Instance.Objects.Remove(Object);
            Object = null;
            HasObject = false;
            NodeGraph.Instance.UpdateGraph(X, Y);
            onTileChanged?.Invoke(this);

            if (shouldCheckForRoom)
            {
                RoomManager.Instance.CheckForRoom(this);
            }
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

        public ItemStack GetItemStack()
        {
            return Item?.ItemStack;
        }

        public TileEnterability GetEnterability()
        {
            return HasObject ? Object.Enterability : TileEnterability.Immediate;
        }

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
            return $"Position: ({X}, {Y})\n" +
                   $"Room: {(Room != null ? Room.RoomID.ToString() : "None")}\n" +
                   $"Zone: {(Zone != null ? Zone.ZoneName : "None")}\n";
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        #endregion

        public override String ToString()
        {
            return $"Tile: {TileDefinition.TileName}   X: {X} Y: {Y}  Obj: {(HasObject ? Object.ObjectName : "None")}";
        }

        public bool Equals(Tile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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
    }
}