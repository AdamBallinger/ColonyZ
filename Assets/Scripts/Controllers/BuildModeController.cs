using Models.Map;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;

namespace Controllers
{
    public enum BuildMode
    {
        Terrain,
        Area,
        Object,
        Demolish,
        Harvest
    }

    public class BuildModeController
    {
        public BuildMode Mode { get; private set; }

        /// <summary>
        /// If the build mode is set to object, this is instance of the object to build
        /// </summary>
        public TileObject Object { get; set; }

        public BuildModeController()
        {
            Mode = BuildMode.Object;
        }

        /// <summary>
        /// Performs a build on a given tile.
        /// TODO: This function in future should create the job based on the build mode, check if placement is valid etc.
        /// </summary>
        /// <param name="_tile"></param>
        public void Build(Tile _tile)
        {
            switch (Mode)
            {
                case BuildMode.Object:
                    HandleObjectBuild(_tile);
                    break;
                case BuildMode.Demolish:
                    _tile.RemoveObject();
                    break;
            }
        }

        private void HandleObjectBuild(Tile _tile)
        {
            if (Object == null)
            {
                return;
            }

            if (World.Instance.IsObjectPositionValid(Object, _tile))
            {
                // TODO: Add Job to build object, rather than this instant build.
                //_tile.SetObject(Object.Clone());
                // Set the tile as a construction base until the job is completed, which should then change the object.
                _tile.SetObject(TileObjectRegistry.GetObject("Construction_Base"));
            }
        }

        public void StartObjectBuild(string _objectName)
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Object;
            Object = TileObjectRegistry.GetObject(_objectName);
        }

        public void StartDemolishBuild()
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Demolish;
        }
    }
}
