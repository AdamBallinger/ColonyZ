using System;
using System.Collections.Generic;
using Controllers;
using Models.Entities.Living;
using Models.Jobs;
using Models.Map.Pathing;
using Models.Map.Tiles.Objects;
using Models.UI;
using UnityEngine;

namespace Models.Map.Tiles
{
    public class Tile : ISelectable
    {
        public int X { get; }
        public int Y { get; }

        public Vector2 Position => new Vector2(X, Y);
        
        /// <summary>
        /// List of living entities currently occupying this tile.
        /// </summary>
        public List<LivingEntity> LivingEntities { get; private set; }
        
        /// <summary>
        /// Reference to any job for this tile.
        /// </summary>
        public Job CurrentJob { get; set; }

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
        /// Contains all neighbours for this tile. (N, NE, E, SE, S, SW, W, NW)
        /// </summary>
        public List<Tile> Neighbours { get; }
        
        /// <summary>
        /// Contains all directly connected neighbours for this tile. (N, E, S, W)
        /// </summary>
        public List<Tile> DirectNeighbours { get; }

        /// <summary>
        /// Determines if the tile has an object installed on it.
        /// </summary>
        public bool HasObject { get; private set; }

        /// <summary>
        /// Installed tile object for this tile.
        /// </summary>
        public TileObject Object { get; private set; }

        private TileDefinition definition, oldDefinition;
        
        private Action<Tile> onTileChanged;
        
        private Action<Tile> onTileDefinitionChanged;

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

        public void SetObject(TileObject _object)
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
            
            onTileChanged?.Invoke(this);
        }

        public void RemoveObject()
        {
            if (!HasObject)
            {
                return;
            }

            World.Instance.Objects.Remove(Object);
            Object = null;
            HasObject = false;
            NodeGraph.Instance.UpdateGraph(X, Y);
            onTileChanged?.Invoke(this);
        }

        public TileEnterability GetEnterability()
        {
            return HasObject ? Object.Enterability : TileEnterability.Immediate;
        }

        /// <summary>
        /// Registers a callback function for when a tile is changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTileChangedCallback(Action<Tile> _callback)
        {
            onTileChanged += _callback;
        }

        /// <summary>
        /// Registers a callback function for when a tiles type is changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTileTypeChangedCallback(Action<Tile> _callback)
        {
            onTileDefinitionChanged += _callback;
        }
        
        #region ISelectable Implementation

        public Sprite GetSelectionIcon()
        {
            return WorldController.Instance.TileTypesSprites[TileDefinition.TextureIndex];
        }

        public string GetSelectionName()
        {
            return TileDefinition.TileName;
        }

        public string GetSelectionDescription()
        {
            return $"Position: ({X}, {Y})\n";
        }
        
        public Vector2 GetPosition()
        {
            return Position;
        }
        
        #endregion
    }
}