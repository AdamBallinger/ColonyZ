using Models.Jobs;
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
            /*if (ObjectToBuild == null)
            {
                return;
            }*/

            if (World.Instance.IsObjectPositionValid(ObjectToBuild, _tile))
            {
                var foundation = Object.Instantiate(TileObjectCache.FoundationObject) as FoundationObject;
                var obj = Object.Instantiate(ObjectToBuild);
                _tile.SetObject(foundation);
                JobManager.Instance.AddJob(new BuildJob(_tile, obj));
            }
        }

        /// <summary>
        /// Sets the controller to build the provided tile object.
        /// </summary>
        /// <param name="_object"></param>
        public void SetBuildMode(TileObject _object)
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Object;
            ObjectToBuild = _object;
        }

        /// <summary>
        /// Sets the controller into demolish mode.
        /// </summary>
        public void SetDemolishMode()
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Demolish;
        }
    }
}
