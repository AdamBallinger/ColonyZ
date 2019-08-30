using Models.Entities.Living;
using Models.Map.Pathing;
using Models.Map.Tiles;
using Models.TimeSystem;
using UnityEngine;

namespace Models.AI
{
    public class AIMotor
    {
        /// <summary>
        /// Determines if the motor is currently moving the entity, or waiting for a path to be returned by the Path finder.
        /// </summary>
        public bool Working { get; private set; }
        
        private LivingEntity Entity { get; }

        /// <summary>
        /// The tile the motor is moving the entity towards.
        /// </summary>
        private Tile targetTile;

        /// <summary>
        /// The path the motor is currently navigating.
        /// </summary>
        private Path path;

        /// <summary>
        /// Stores the distance between the entities current tile and the next tile in the path.
        /// </summary>
        private float distance;

        /// <summary>
        /// Ranging from 0 to 1, the percentage progress of the movement from the current tile to the next tile in the path.
        /// </summary>
        private float travelProgress;
        
        public AIMotor(LivingEntity _entity)
        {
            Working = false;
            Entity = _entity;
        }
        
        /// <summary>
        /// Update the tile the motor moves the entity towards.
        /// </summary>
        /// <param name="_tile"></param>
        public void SetTargetTile(Tile _tile)
        {
            // Don't try move to the same tile the entity is currently on.
            if (Entity.CurrentTile.Position == _tile.Position)
            {
                return;
            }

            Working = true;
            targetTile = _tile;
            PathFinder.NewRequest(Entity.CurrentTile, targetTile, OnPathReceived);
        }
        
        public void Stop()
        {
            // TODO: Find a way to end a path without breaking the motor or causing the entity to teleport because of the tile offset.
        }
        
        public void Update()
        {
            if (path == null)
            {
                return;
            }

            if (!path.IsValid)
            {
                // Find a new path if the current one was invalidated.
                FinishPath();
                SetTargetTile(targetTile);
                return;
            }

            // The amount the entity will move this frame.
            var movementDelta = Entity.MovementSpeed * path.CurrentTile.TileDefinition.MovementModifier * TimeManager.Instance.DeltaTime;
            var travelPercentageThisFrame = movementDelta / distance;
            travelProgress += travelPercentageThisFrame;
            
            if (travelProgress >= 1.0f)
            {
                Entity.CurrentTile = path.CurrentTile;
                path.Next();
                
                // Check if at the end of the path.
                if (path.CurrentTile == null)
                {
                    FinishPath();
                    return;
                }

                distance = GetDistance();

                var overlap = Mathf.Clamp01(travelProgress - 1.0f);
                travelProgress = overlap;
                
                // TODO: only set travel progress to overlap if the next tile in the path is actually pathable. Otherwise path ends.
            }

            Entity.TileOffset = (path.CurrentTile.Position - Entity.CurrentTile.Position) * travelProgress;
        }
        
        private void OnPathReceived(Path _path)
        {
            if (_path.IsValid && _path.Size > 0)
            {
                Working = true;
                path = _path;
                // Get the distance between the entities current tile and the next tile in the path.
                distance = GetDistance();
                travelProgress = 0.0f;
            }
            else
            {
                FinishPath();
            }
        }
        
        private void FinishPath()
        {
            Working = false;
            path = null;
            travelProgress = 0.0f;
            Entity.TileOffset = Vector2.zero;
        }
        
        private float GetDistance()
        {
            return Mathf.Sqrt(Mathf.Pow(Entity.CurrentTile.X - path.CurrentTile.X, 2) +
                              Mathf.Pow(Entity.CurrentTile.Y - path.CurrentTile.Y, 2));
        }
    }
}