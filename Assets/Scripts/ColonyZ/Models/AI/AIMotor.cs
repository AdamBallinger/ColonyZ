using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
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
        
        /// <summary>
        ///     Curve controlling rate of movement between each path position.
        /// </summary>
        private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        /// <summary>
        ///     T value used to evaluate movement curve.
        /// </summary>
        private float interpolationTime;
        
        /// <summary>
        ///     Position of previous path point the entity is moving from.
        /// </summary>
        private Vector2 originPos;

        /// <summary>
        ///     Distance between the last and current path position.
        /// </summary>
        private float travelDistance;

        /// <summary>
        ///     Determines if the motor should wait to move the entity.
        /// </summary>
        private bool shouldWait;

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

            var dt = TimeManager.Instance.DeltaTime;
            var entityMovementSpeed = Entity.MovementSpeed * Entity.CurrentTile.TileDefinition.MovementModifier;

            interpolationTime += shouldWait ? 0.0f : dt / (travelDistance / entityMovementSpeed);

            Entity.SetPosition(Vector2.Lerp(originPos, path.Current, movementCurve.Evaluate(interpolationTime)));

            if (interpolationTime >= 0.5f)
            {
                var tile = World.Instance.GetTileAt(path.Current);
                if (tile.GetEnterability() == TileEnterability.Delayed)
                {
                    var door = tile.Object as DoorObject;
                    if (!door.IsOpen)
                    {
                        shouldWait = true;
                        door.OpenDoor();
                        return;
                    }

                    shouldWait = false;
                }
            }

            if (interpolationTime >= 1.0f)
            {
                if (path.IsLastPoint)
                {
                    path.Next();
                    FinishPath();
                    return;
                }
                
                interpolationTime = 0.0f;
                path.Next();
                originPos = Entity.Position;
                travelDistance = Vector2.Distance(originPos, path.Current);
            }
            
            var direction = (path.Current - Entity.Position).normalized;
            CalculateMotorDirection(Vector2.Dot(Vector2.up, direction));
        }

        public void FaceTile(Tile _tile)
        {
            if (_tile == null) return;

            var entityX = Entity.CurrentTile.X;
            var entityY = Entity.CurrentTile.Y;

            if (entityX == _tile.X)
            {
                MotorDirection = entityY < _tile.Y ? AIMotorDirection.Up : AIMotorDirection.Down;
            }

            if (entityY == _tile.Y)
            {
                MotorDirection = entityX < _tile.X ? AIMotorDirection.Right : AIMotorDirection.Left;
            }
        }

        private void CalculateMotorDirection(float _angle)
        {
            if (_angle > 0.75f)
            {
                MotorDirection = AIMotorDirection.Up;
            }
            else if (_angle > 0.0f)
            {
                MotorDirection = path.Current.x < Entity.X ? AIMotorDirection.Left : AIMotorDirection.Right;
            }
            else if (_angle < -0.75f)
            {
                MotorDirection = AIMotorDirection.Down;
            }
            else if (_angle < 0.0f)
            {
                MotorDirection = path.Current.x < Entity.X ? AIMotorDirection.Left : AIMotorDirection.Right;
            }
            else if (_angle == 0.0f)
            {
                MotorDirection = path.Current.x < Entity.X ? AIMotorDirection.Left : AIMotorDirection.Right;
            }
        }

        private void OnPathReceived(Path _path)
        {
            if (_path != null && _path.IsValid && _path.SmoothSize > 0)
            {
                Working = true;
                path = _path;
                originPos = Entity.Position;
                interpolationTime = 0.0f;
                travelDistance = Vector2.Distance(originPos, path.Current);
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
        }
    }
}