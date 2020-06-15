using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.TimeSystem;
using UnityEngine;

namespace ColonyZ.Models.AI
{
    public class AIMotor
    {
        /// <summary>
        ///     The path the motor is currently navigating.
        /// </summary>
        public Path path;

        /// <summary>
        ///     Determines if the motor is currently moving the entity, or waiting for a path to be returned by the Path finder.
        /// </summary>
        public bool Working { get; private set; }

        /// <summary>
        ///     The direction that the motor is currently moving in.
        /// </summary>
        public AIMotorDirection MotorDirection { get; private set; }

        /// <summary>
        ///     The tile the motor is moving the entity towards.
        /// </summary>
        private Tile TargetTile { get; set; }

        private LivingEntity Entity { get; }

        public AIMotor(LivingEntity _entity)
        {
            Working = false;
            Entity = _entity;
        }

        /// <summary>
        ///     Update the tile the motor moves the entity towards.
        /// </summary>
        /// <param name="_tile"></param>
        public void SetTargetTile(Tile _tile)
        {
            FinishPath();

            if (_tile.GetEnterability() == TileEnterability.None) return;

            // TODO: Change back to area system when area detection is faster.
            if (!RegionReachabilityChecker.CanReachRegion(Entity.CurrentTile.Region, _tile.Region))
                return;

            Working = true;
            TargetTile = _tile;
            PathFinder.NewRequest(Entity.CurrentTile, TargetTile, OnPathReceived);
        }

        public void Update()
        {
            if (path == null)
                return;

            if (!path.IsValid)
            {
                // Find a new path if the current one was invalidated.
                FinishPath();
                SetTargetTile(TargetTile);
                return;
            }

            var dist = Vector2.Distance(Entity.Position, path.CurrentTile.Position);
            var dt = TimeManager.Instance.DeltaTime;
            var dir = (path.CurrentTile.Position - Entity.Position).normalized;

            if (dist <= 0.0f)
            {
                // If the tile we were pathing to was the last tile in the path.
                if (path.LastTile)
                {
                    FinishPath();
                    return;
                }

                path.Next();
                return;
            }

            if (dir.x == -1.0f) MotorDirection = AIMotorDirection.Left;
            else if (dir.x == 1.0f) MotorDirection = AIMotorDirection.Right;
            else if (dir.y > 0.0f) MotorDirection = AIMotorDirection.Up;
            else MotorDirection = AIMotorDirection.Down;

            // The rate in which we move the entity this frame.
            var movementDelta = Entity.MovementSpeed * path.CurrentTile.TileDefinition.MovementModifier * dt;

            var position = Vector2.MoveTowards(Entity.Position, Entity.Position + dir * dist, movementDelta);
            Entity.SetPosition(position);
        }

        private void OnPathReceived(Path _path)
        {
            if (_path.IsValid && _path.Size > 0)
            {
                Working = true;
                path = _path;
            }
            else
            {
                FinishPath();
                Entity.OnPathFailed();
            }
        }

        public void FinishPath()
        {
            Working = false;
            path = null;
            MotorDirection = 0;
        }
    }
}