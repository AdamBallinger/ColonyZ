using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;

namespace ColonyZ.Controllers
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
    ///     TODO: This class is become a shit show and should be re done at some point..
    /// </summary>
    public class BuildModeController
    {
        private bool _godMode;

        public BuildModeController()
        {
            Mode = BuildMode.Object;
            GodMode = World.Instance.WorldProvider.GodMode;
        }

        public BuildMode Mode { get; private set; }

        public bool GodMode
        {
            get => _godMode;
            set
            {
                if (value != _godMode)
                {
                    _godMode = value;
                    godModeChangeEvent?.Invoke(_godMode);
                }
            }
        }

        /// <summary>
        ///     Reference to the object that will be built on a tile when in Object build mode.
        /// </summary>
        public TileObject ObjectToBuild { get; private set; }

        public Zone ZoneToBuild { get; private set; }

        public event Action<bool> godModeChangeEvent;

        /// <summary>
        ///     Processes the given tiles based on the current mode.
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

                    if (ZoneToBuild.MinimumSize.x <= _width && ZoneToBuild.MinimumSize.y <= _height)
                        HandleBuildArea(_tiles, _x, _y, _width, _height);

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
                if (!ZoneToBuild.CanPlace(tile))
                    return;

            ZoneToBuild.SetOrigin(_x, _y);
            ZoneToBuild.SetSize(_width, _height);

            ZoneManager.Instance.AddZone(ZoneToBuild);

            // Create new template instance for the zone being built.
            ZoneToBuild = (Zone) Activator.CreateInstance(ZoneToBuild.GetType());
        }

        private void HandleBuild(IEnumerable<Tile> _tiles)
        {
            var jobs = new List<Job>();

            foreach (var tile in _tiles)
                if (World.Instance.IsObjectPositionValid(ObjectToBuild, tile))
                {
                    var obj = TileObjectCache.GetObject(ObjectToBuild);
                    if (GodMode)
                    {
                        tile.SetObject(obj);
                    }
                    else
                    {
                        var foundation = TileObjectCache.GetFoundation();
                        tile.SetObject(foundation);
                        jobs.Add(new BuildJob(tile, obj));
                    }
                }

            JobManager.Instance.AddJobs(jobs);
        }

        private void HandleDemolish(IEnumerable<Tile> _tiles)
        {
            if (GodMode)
            {
                foreach (var tile in _tiles)
                    if (tile.HasObject && ObjectCompatWithMode(tile.Object))
                    {
                        tile.RemoveObject();

                        if (tile.CurrentJob != null)
                        {
                            JobManager.Instance.DeleteJob(tile.CurrentJob);
                        }
                    }

                return;
            }

            var jobs = (from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object)
                select new DemolishJob(tile));

            JobManager.Instance.AddJobs(jobs);
        }

        private void HandleGather(IEnumerable<Tile> _tiles)
        {
            if (GodMode)
            {
                foreach (var tile in _tiles)
                    if (tile.HasObject && ObjectCompatWithMode(tile.Object))
                    {
                        tile.RemoveObject();

                        if (tile.CurrentJob != null)
                        {
                            JobManager.Instance.DeleteJob(tile.CurrentJob);
                        }
                    }

                return;
            }

            var jobs = (from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object)
                select new HarvestJob(tile, Mode.ToString()));

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

        public void SetZone(Zone _zoneToBuild)
        {
            MouseController.Instance.Mode = MouseMode.Process;
            Mode = BuildMode.Area;
            ZoneToBuild = _zoneToBuild;
        }

        /// <summary>
        ///     Sets the controller to build the provided tile object.
        /// </summary>
        /// <param name="_object"></param>
        public void SetBuildMode(TileObject _object)
        {
            MouseController.Instance.Mode = _object.Draggable ? MouseMode.Process : MouseMode.Process_Single;
            Mode = BuildMode.Object;
            ObjectToBuild = _object;
        }

        /// <summary>
        ///     Sets the controller into demolish mode.
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