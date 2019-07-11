using Models.Map;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using UnityEngine;

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
        /// Reference to the object that will be built on a tile when in Object build mode.
        /// </summary>
        public TileObject ObjectToBuild { get; set; }

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
            if (ObjectToBuild == null)
            {
                return;
            }

            if (World.Instance.IsObjectPositionValid(ObjectToBuild, _tile))
            {
                // TODO: Add Job to build object, rather than this instant build.
                // Set the tile as a construction base until the job is completed, which should then change the object.
                _tile.SetObject(Object.Instantiate(ObjectToBuild));
            }
        }

        public void StartObjectBuild(TileObject _object)
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Object;
            ObjectToBuild = _object;
        }

        public void StartDemolishBuild()
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Demolish;
        }
    }
}
