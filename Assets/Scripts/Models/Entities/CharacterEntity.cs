using Models.Map;
using Models.Pathing;
using UnityEngine;

namespace Models.Entities
{
    public class CharacterEntity : Entity
    {

        public float MovementSpeed { get; protected set; }

        private Path path;

        private Tile nextTile;

        private float movementPercentage;
        private float distToTravel;
        private float distThisFrame;
        private float percentThisFrame;
        private float overShotAmount;

        public CharacterEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 1.0f;
            PathFinished();
        }

        public override void Update()
        {
            // TODO: Move movement into a state system later on.
            if (path == null) return;

            if (nextTile?.GetEnterability() == Enterability.Delayed)
            {
                // Cant walk through tile yet so wait. Doesn't ever get occur yet however.
                return;
            }
            
            // TODO: Change Time.deltaTime to a custom Time tracking class.
            distThisFrame = MovementSpeed * nextTile.MovementModifier * Time.deltaTime;
            percentThisFrame = distThisFrame / distToTravel;
            movementPercentage += percentThisFrame;

            if (movementPercentage >= 1.0f)
            {
                // Next tile in path reached.
                CurrentTile = nextTile;

                // Track how much the character went over its last distance to the current tile to smoothly transition to the next tile in the path.
                overShotAmount = Mathf.Clamp01(movementPercentage - 1.0f);
                movementPercentage = 0.0f;

                if (HasReachedPathDestination())
                {
                    PathFinished();
                    return;
                }

                AdvancePath();

                distToTravel = Mathf.Sqrt(Mathf.Pow(CurrentTile.X - nextTile.X, 2) + Mathf.Pow(CurrentTile.Y - nextTile.Y, 2));

                switch (nextTile.GetEnterability())
                {
                    case Enterability.Immediate:
                        movementPercentage = overShotAmount;
                        break;
                    case Enterability.None:
                        // Abort because something got in the way like a new structure.
                        // TODO: Possibly recalcualte the path?
                        Debug.Log("Path failed to complete for character.");
                        PathFinished();
                        return;
                }
            }

            TileOffset = new Vector2((nextTile.X - CurrentTile.X) * movementPercentage, (nextTile.Y - CurrentTile.Y) * movementPercentage);
        }

        private void PathFinished()
        {
            path = null;
            TileOffset = Vector2.zero;
            PathFinder.NewRequest(CurrentTile, World.Instance?.GetRandomTile(0, 0, 5, 5), OnPathReceived);
        }

        private void OnPathReceived(Path _path)
        {
            if (_path.IsValid && _path.Size > 1)
            {
                path = _path;
                path.Next(); // Remove the current tile character is on from the path.
                AdvancePath();
                distToTravel = Mathf.Sqrt(Mathf.Pow(CurrentTile.X - nextTile.X, 2) + Mathf.Pow(CurrentTile.Y - nextTile.Y, 2));
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

        private bool HasReachedPathDestination()
        {
            return path?.Size == 0 || path.EndTile == CurrentTile;
        }
    }
}
