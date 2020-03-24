using System.Collections.Generic;
using System.Linq;
using Models.AI.Jobs;
using Models.Map;
using Models.Map.Areas;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using UnityEngine;

namespace Controllers
{
    public enum BuildMode
    {
        Area,
        Object,
        Demolish,
        Fell,
        Mine,
        Harvest
    }

    /// <summary>
    /// TODO: This class is become a shit show and should be re done at some point..
    /// </summary>
    public class BuildModeController
    {
        public BuildMode Mode { get; private set; }

        /// <summary>
        /// Reference to the object that will be built on a tile when in Object build mode.
        /// </summary>
        public TileObject ObjectToBuild { get; private set; }

        public Area AreaToBuild { get; private set; }

        public BuildModeController()
        {
            Mode = BuildMode.Object;
        }

        /// <summary>
        /// Processes the given tiles based on the current mode.
        /// </summary>
        /// <param name="_tiles"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public void Process(IEnumerable<Tile> _tiles, int _x = 0, int _y = 0, int _width = 0, int _height = 0)
        {
            switch (Mode)
            {
                case BuildMode.Area:

                    if (AreaToBuild.MinimumSize.x <= _width && AreaToBuild.MinimumSize.y <= _height)
                    {
                        HandleBuildArea(_tiles, _x, _y, _width, _height);
                    }

                    break;
                case BuildMode.Object:
                    HandleBuild(_tiles);
                    break;
                case BuildMode.Demolish:
                    HandleDemolish(_tiles);
                    break;
                case BuildMode.Mine:
                case BuildMode.Fell:
                case BuildMode.Harvest:
                    HandleGather(_tiles);
                    break;
            }
        }

        private void HandleBuildArea(IEnumerable<Tile> _tiles, int _x, int _y, int _width, int _height)
        {
            var enumerable = _tiles.ToArray();

            foreach (var tile in enumerable)
            {
                if (AreaToBuild.CanPlace(tile))
                {
                    return;
                }
            }

            AreaToBuild.SetOrigin(_x, _y); // TODO: Is this even needed?
            AreaToBuild.SetSize(_width, _height);

            foreach (var tile in enumerable)
            {
                tile.Area = AreaToBuild;
            }
        }

        private void HandleBuild(IEnumerable<Tile> _tiles)
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

        private void HandleDemolish(IEnumerable<Tile> _tiles)
        {
            var jobs = (from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object)
                select new DemolishJob(tile)).Cast<Job>().ToList();

            JobManager.Instance.AddJobs(jobs);
        }

        private void HandleGather(IEnumerable<Tile> _tiles)
        {
            var jobs = (from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object)
                select new HarvestJob(tile, Mode.ToString())).Cast<Job>().ToList();

            JobManager.Instance.AddJobs(jobs);
        }

        private bool ObjectCompatWithMode(TileObject _object)
        {
            switch (Mode)
            {
                case BuildMode.Demolish:
                    return _object.Buildable;
                case BuildMode.Mine:
                    return _object.Mineable;
                case BuildMode.Fell:
                    return _object.Fellable;
                case BuildMode.Harvest:
                    return _object.Harvestable;
            }

            return false;
        }

        public void SetAreaMode(Area _areaToBuild)
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Area;
            AreaToBuild = _areaToBuild;
        }

        /// <summary>
        /// Sets the controller to build the provided tile object.
        /// </summary>
        /// <param name="_object"></param>
        public void SetBuildMode(TileObject _object)
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Object;
            ObjectToBuild = _object;
        }

        /// <summary>
        /// Sets the controller into demolish mode.
        /// </summary>
        public void SetDemolishMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Demolish;
        }

        public void SetFellMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Fell;
        }

        public void SetMineMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Mine;
        }

        public void SetHarvestMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Harvest;
        }
    }
}