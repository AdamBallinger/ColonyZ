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
                    HandleObjectDemolish(_tile);
                    break;
            }
        }

        private void HandleObjectBuild(Tile _tile)
        {
            if (World.Instance.IsObjectPositionValid(ObjectToBuild, _tile))
            {
                var foundation = Object.Instantiate(TileObjectCache.FoundationObject) as FoundationObject;
                var obj = Object.Instantiate(ObjectToBuild);
                _tile.SetObject(foundation);
                JobManager.Instance.AddJob(new BuildJob(_tile, obj));
            }
        }
        
        private void HandleObjectDemolish(Tile _tile)
        {
            if (_tile.Object != null)
            {
                JobManager.Instance.AddJob(new DemolishJob(_tile));
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
