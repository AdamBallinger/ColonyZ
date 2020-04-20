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

        /// <summary>
        /// The directional index that the motor is currently moving in.
        /// 0 - Moving Down / Not moving
        /// 1 - Moving Right
        /// 2 - Moving Left
        /// 3 - Moving Up
        /// </summary>
        public int DirectionIndex { get; private set; } = 0;

        /// <summary>
        /// The tile the motor is moving the entity towards.
        /// </summary>
        public Tile TargetTile { get; private set; }

        private LivingEntity Entity { get; }

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
            Stop();

            // Don't try move to the same tile the entity is currently on.
            if (Entity.CurrentTile.Position == _tile.Position)
            {
                return;
            }

            // If the entities current area has no connection to the targets room,
            // then we already know there's no valid path.
            // if (Entity.CurrentTile.Area != null && !Entity.CurrentTile.Area.HasConnection(_tile.Area))
            // {
            //     return;
            // }

            Working = true;
            TargetTile = _tile;
            PathFinder.NewRequest(Entity.CurrentTile, TargetTile, OnPathReceived);
        }

        public void Stop()
        {
            // TODO: Find a way to end a path without breaking the motor or causing the entity to teleport because of the tile offset.
            FinishPath();
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
                SetTargetTile(TargetTile);
                return;
            }

            var pathX = path.CurrentTile.X;
            var pathY = path.CurrentTile.Y;
            var entityX = Entity.CurrentTile.X;
            var entityY = Entity.CurrentTile.Y;

            if (pathY < entityY) // Moving down
            {
                DirectionIndex = pathX == entityX ? 0 : pathX > entityX ? 1 : 2;
            }
            else if (pathY > entityY) // Moving up
            {
                DirectionIndex = pathX == entityX ? 3 : pathX > entityX ? 1 : 2;
            }

            // The amount the entity will move this frame.
            var movementDelta = Entity.MovementSpeed * path.CurrentTile.TileDefinition.MovementModifier *
                                TimeManager.Instance.DeltaTime;
            var travelPercentageThisFrame = movementDelta / distance;
            travelProgress += travelPercentageThisFrame;

            if (travelProgress >= 1.0f)
            {
                Entity.CurrentTile.LivingEntities.Remove(Entity);
                Entity.CurrentTile = path.CurrentTile;
                Entity.CurrentTile.LivingEntities.Add(Entity);
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
                Entity.OnPathFailed();
            }
        }

        private void FinishPath()
        {
            Working = false;
            path = null;
            travelProgress = 0.0f;
            Entity.TileOffset = Vector2.zero;
            DirectionIndex = 0;
        }

        private float GetDistance()
        {
            return Mathf.Sqrt(Mathf.Pow(Entity.CurrentTile.X - path.CurrentTile.X, 2) +
                              Mathf.Pow(Entity.CurrentTile.Y - path.CurrentTile.Y, 2));
        }
    }
}