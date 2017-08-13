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

        public CharacterEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 5.0f;
            PathFinder.NewRequest(CurrentTile, World.Instance?.GetRandomTile(), OnPathReceived);
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
            var distThisFrame = MovementSpeed * nextTile.MovementModifier * Time.deltaTime;
            var percentThisFrame = distThisFrame / distToTravel;
            movementPercentage += percentThisFrame;

            if (movementPercentage >= 1.0f)
            {
                // Next tile in path reached.
                CurrentTile = nextTile;

                // Track how much the character went over its last distance to the current tile to smoothly transition to the next tile in the path.
                var overShotAmount = Mathf.Clamp01(movementPercentage - 1.0f);
                movementPercentage = 0.0f;

                if (HasReachedPathDestination())
                {
                    // TODO: Maybe do something here.
                    path = null;
                    PathFinder.NewRequest(CurrentTile, World.Instance?.GetRandomTile(), OnPathReceived);
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
                        path = null;
                        return;
                }
            }

            TileOffset = new Vector2((nextTile.X - CurrentTile.X) * movementPercentage, (nextTile.Y - CurrentTile.Y) * movementPercentage);
        }

        private void OnPathReceived(Path _path)
        {
            if(_path.IsValid)
            {
                path = _path;
                AdvancePath();
            }
        }

        private void AdvancePath()
        {
            nextTile = World.Instance?.GetTileAt(path.NodePath[0].X, path.NodePath[0].Y);
            path.NodePath.RemoveAt(0);
        }

        private bool HasReachedPathDestination()
        {
            return path?.NodePath.Count == 0 || path.EndNode.X == CurrentTile.X && path.EndNode.Y == CurrentTile.Y ? true : false;
        }
    }
}
