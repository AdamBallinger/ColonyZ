using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
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
            if (Entity.Position == _tile.Position) return;

            // Don't make a path request if the current area the entity is in has no link to the target area.
            // Checks that the entities current area isn't null to ensure that entities that get stuck inside of
            // objects can get out.
            if (Entity.CurrentTile.Area != null && !Entity.CurrentTile.Area.HasConnection(_tile.Area))
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

            var dist = Vector2.Distance(Entity.Position, path.Current);

            // Allow small tolerance for floating point precision.
            if (dist <= 0.0001f)
            {
                // Set the entity position to make sure it is not off by a small value due to precision issues.
                Entity.SetPosition(path.Current);

                // If the tile we were pathing to was the last tile in the path, then the path is finished.
                if (path.IsLastPoint)
                {
                    Entity.SetPosition(World.Instance.GetTileAt(path.Current).Position);
                    // Move next so that the path is properly removed from the node at that tile.
                    path.Next();
                    FinishPath();
                    return;
                }

                path.Next();
                dist = Vector2.Distance(Entity.Position, path.Current);
            }

            var dt = TimeManager.Instance.DeltaTime;
            var dir = (path.Current - Entity.Position).normalized;

            var dirAngle = Vector2.Dot(Vector2.up, dir);

            if (dirAngle > 0.75f)
            {
                MotorDirection = AIMotorDirection.Up;
            }
            else if (dirAngle > 0.0f)
            {
                MotorDirection = path.Current.x < Entity.X ? AIMotorDirection.Left : AIMotorDirection.Right;
            }
            else if (dirAngle < -0.75f)
            {
                MotorDirection = AIMotorDirection.Down;
            }
            else if (dirAngle < 0.0f)
            {
                MotorDirection = path.Current.x < Entity.X ? AIMotorDirection.Left : AIMotorDirection.Right;
            }
            else if (dirAngle == 0.0f)
            {
                MotorDirection = path.Current.x < Entity.X ? AIMotorDirection.Left : AIMotorDirection.Right;
            }

            // The rate in which we move the entity this frame.
            var movementDelta = Entity.MovementSpeed * Entity.CurrentTile.TileDefinition.MovementModifier * dt;

            var position = Vector2.MoveTowards(Entity.Position, Entity.Position + dir * dist, movementDelta);
            Entity.SetPosition(position);
        }

        private void OnPathReceived(Path _path)
        {
            if (_path.IsValid && _path.SmoothSize > 0)
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
            //MotorDirection = 0;
        }
    }
}