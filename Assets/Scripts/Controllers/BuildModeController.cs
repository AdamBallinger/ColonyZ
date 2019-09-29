using System.Collections.Generic;
using System.Linq;
using Models.Jobs;
using Models.Map;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using UnityEngine;

namespace Controllers
{
    public enum BuildMode
    {
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
        /// Processes the given tiles based on the current mode.
        /// </summary>
        /// <param name="_tiles"></param>
        public void Process(IEnumerable<Tile> _tiles)
        {
            switch (Mode)
            {
                case BuildMode.Object:
                    HandleObjectBuild(_tiles);
                    break;
                case BuildMode.Demolish:
                    HandleObjectDemolish(_tiles);
                    break;
                case BuildMode.Harvest:
                    HandleHarvest(_tiles);
                    break;
            }
        }

        private void HandleObjectBuild(IEnumerable<Tile> _tiles)
        {
            var jobs = new List<Job>();
            
            foreach (var tile in _tiles)
            {
                if (World.Instance.IsObjectPositionValid(ObjectToBuild, tile))
                {
                    var foundation = Object.Instantiate(TileObjectCache.FoundationObject) as FoundationObject;
                    var obj = Object.Instantiate(ObjectToBuild);
                    tile.SetObject(foundation);
                    jobs.Add(new BuildJob(tile, obj));
                }
            }
            
            JobManager.Instance.AddJobs(jobs);
        }
        
        private void HandleObjectDemolish(IEnumerable<Tile> _tiles)
        {
            var jobs = (from tile in _tiles where tile.HasObject select new DemolishJob(tile)).Cast<Job>().ToList();

            JobManager.Instance.AddJobs(jobs);
        }
        
        private void HandleHarvest(IEnumerable<Tile> _tiles)
        {
            var jobs = (from tile in _tiles where tile.HasObject && tile.Object is NatureObject 
                        select new HarvestJob(tile)).Cast<Job>().ToList();
            
            JobManager.Instance.AddJobs(jobs);
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
        
        public void SetHarvestMode()
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Harvest;
        }
    }
}
