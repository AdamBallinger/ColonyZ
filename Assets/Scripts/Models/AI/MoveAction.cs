using Models.Entities;
using Models.Map;
using Models.Map.Pathing;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.AI
{
    public class MoveAction : BaseAction
    {
        private CharacterEntity character;

        private Tile destination;

        private Path path;

        private Tile nextTile;

        private float movementPercentage;
        private float distToTravel;
        private float distThisFrame;
        private float percentThisFrame;
        private float overShotAmount;

        public MoveAction(CharacterEntity _character, Tile _destination)
        {
            character = _character;
            destination = _destination;
            PathFinder.NewRequest(character.CurrentTile, destination, OnPathReceived);
        }

        public override void OnUpdate()
        {
            if (path == null) return;

            if (nextTile?.GetEnterability() == Enterability.Delayed)
            {
                // Cant walk through tile yet so wait. TODO: Implement this when its a thing!
                return;
            }

            // TODO: Change Time.deltaTime to a custom Time tracking class.
            distThisFrame = character.MovementSpeed * nextTile.MovementCost * Time.deltaTime;
            percentThisFrame = distThisFrame / distToTravel;
            movementPercentage += percentThisFrame;

            if (movementPercentage >= 1.0f)
            {
                // Next tile in path reached.
                character.CurrentTile = nextTile;

                // Track how much the character went over its last distance to the current tile to smoothly transition to the next tile in the path.
                overShotAmount = Mathf.Clamp01(movementPercentage - 1.0f);
                movementPercentage = 0.0f;

                if (HasReachedPathDestination())
                {
                    PathFinished();
                    return;
                }

                AdvancePath();

                distToTravel = Mathf.Sqrt(Mathf.Pow(character.CurrentTile.X - nextTile.X, 2) + 
                                          Mathf.Pow(character.CurrentTile.Y - nextTile.Y, 2));

                switch (nextTile.GetEnterability())
                {
                    case Enterability.Immediate:
                        movementPercentage = overShotAmount;
                        break;
                    case Enterability.None:
                        // If the next tile can't be entered then something was built after the path was
                        // created, so recalculate the path to the destination from the current tile.
                        //TODO: recalculate path when a tile in the path has changed rather than waiting for the entity to get there.
                        PathFinder.NewRequest(character.CurrentTile, destination, OnPathReceived);
                        PathFinished();
                        return;
                }
            }

            character.TileOffset = new Vector2((nextTile.X - character.CurrentTile.X) * movementPercentage,
                (nextTile.Y - character.CurrentTile.Y) * movementPercentage);
        }

        private void OnPathReceived(Path _path)
        {
            if (_path.IsValid && _path.Size > 1)
            {
                path = _path;
                path.Next(); // Remove the current tile the character is on from the path.
                AdvancePath();
                distToTravel = Mathf.Sqrt(Mathf.Pow(character.CurrentTile.X - nextTile.X, 2) + 
                                          Mathf.Pow(character.CurrentTile.Y - nextTile.Y, 2));
            }
            else
            {
                PathFinished();
            }
        }

        private void AdvancePath()
        {
            nextTile = path?.Next();
        }

        private void PathFinished()
        {
            path = null;
            character.TileOffset = Vector2.zero;
        }

        private float CalculatePathDistance()
        {
            return 0.0f;
        }

        private bool HasReachedPathDestination()
        {
            return path?.Size == 0 || path.EndTile == character.CurrentTile;
        }

        public override bool HasFinished()
        {
            return path != null && HasReachedPathDestination();
        }
    }
}
