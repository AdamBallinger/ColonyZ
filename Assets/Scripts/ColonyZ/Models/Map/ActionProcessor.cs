using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Controllers;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Utils;

namespace ColonyZ.Models.Map
{
    public enum ProcessMode
    {
        Zone,
        Object,
        Demolish,
        Gather,
        Cancel
    }

    public enum GatherMode
    {
        Fell,
        Mine,
        Harvest
    }

    /// <summary>
    /// Class for handling the processing of building, demolishing etc.
    /// </summary>
    public class ActionProcessor
    {
        /// <summary>
        /// Current proc
        /// </summary>
        public ProcessMode ProcessMode { get; private set; }

        public GatherMode GatherMode { get; private set; }

        /// <summary>
        ///     Reference to the object that will be built on a tile when in Object work mode.
        /// </summary>
        public TileObjectData ObjectToBuild { get; private set; }

        public Zone ZoneToBuild { get; private set; }

        public event Action<bool> godModeChangeEvent;

        public bool GodMode
        {
            get => _godMode;
            private set
            {
                if (value != _godMode)
                {
                    _godMode = value;
                    godModeChangeEvent?.Invoke(_godMode);
                }
            }
        }

        private bool _godMode;

        public ActionProcessor()
        {
            ProcessMode = ProcessMode.Object;
            GatherMode = GatherMode.Mine;
            GodMode = World.Instance.WorldProvider.GodMode;
        }

        public void ToggleGodMode()
        {
            GodMode = !GodMode;
        }

        /// <summary>
        ///     Processes the given tiles based on the current mode.
        /// </summary>
        /// <param name="_tiles"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_rotation"></param>
        public void Process(IEnumerable<Tile> _tiles, int _x = 0, int _y = 0, int _width = 0, int _height = 0, 
            ObjectRotation _rotation = ObjectRotation.Default)
        {
            switch (ProcessMode)
            {
                case ProcessMode.Zone:
                    if (ZoneToBuild.MinimumSize.x <= _width && ZoneToBuild.MinimumSize.y <= _height)
                        HandleBuildZone(_tiles, _x, _y, _width, _height);
                    break;
                case ProcessMode.Object:
                    HandleBuild(_tiles, _rotation);
                    break;
                case ProcessMode.Demolish:
                    HandleDemolish(_tiles);
                    break;
                case ProcessMode.Gather:
                    HandleGather(_tiles);
                    break;
                case ProcessMode.Cancel:
                    HandleCancel(_tiles);
                    break;
            }
        }

        private void HandleBuildZone(IEnumerable<Tile> _tiles, int _x, int _y, int _width, int _height)
        {
            var enumerable = _tiles.ToArray();

            foreach (var tile in enumerable)
            {
                if (!ZoneToBuild.CanPlace(tile))
                    return;
            }

            ZoneToBuild.SetOrigin(_x, _y);
            ZoneToBuild.SetSize(_width, _height);

            ZoneManager.Instance.AddZone(ZoneToBuild);

            // Create new template instance for the zone being built.
            ZoneToBuild = (Zone) Activator.CreateInstance(ZoneToBuild.GetType());
        }

        private void HandleBuild(IEnumerable<Tile> _tiles, ObjectRotation _rotation)
        {
            var jobs = new List<Job>();
            var factory = ObjectFactoryUtil.GetFactory(ObjectToBuild.FactoryType);

            foreach (var tile in _tiles)
                if (World.Instance.IsObjectPositionValid(ObjectToBuild, tile, _rotation))
                {
                    var obj = factory.GetObject(ObjectToBuild);
                    obj.ObjectRotation = _rotation;
                    if (GodMode)
                    {
                        tile.SetObject(obj);
                    }
                    else
                    {
                        var foundation = TileObjectDataCache.GetFoundation();
                        var foundationObj = new FoundationObject(foundation, ObjectToBuild, _rotation, tile);
                        tile.SetObject(foundationObj);
                        foundationObj.PlaceFullFoundation();
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
                {
                    if (tile.HasObject && ObjectCompatWithMode(tile.Object.ObjectData))
                    {
                        tile.RemoveObject();

                        if (tile.CurrentJob != null)
                        {
                            JobManager.Instance.CancelJob(tile.CurrentJob);
                        }
                    }
                }

                return;
            }

            var jobs = (from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object.ObjectData)
                select new DemolishJob(tile));

            JobManager.Instance.AddJobs(jobs);
        }

        private void HandleGather(IEnumerable<Tile> _tiles)
        {
            if (GodMode)
            {
                foreach (var tile in _tiles)
                {
                    if (tile.HasObject && ObjectCompatWithMode(tile.Object.ObjectData))
                    {
                        tile.RemoveObject();

                        if (tile.CurrentJob != null)
                        {
                            JobManager.Instance.CancelJob(tile.CurrentJob);
                        }
                    }
                }

                return;
            }

            var jobs = (from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object.ObjectData)
                select new HarvestJob(tile, ProcessMode.ToString()));

            JobManager.Instance.AddJobs(jobs);
        }

        private void HandleCancel(IEnumerable<Tile> _tiles)
        {
            foreach (var tile in _tiles)
            {
                if (tile.CurrentJob != null && !tile.CurrentJob.Complete)
                {
                    JobManager.Instance.CancelJob(tile.CurrentJob);
                }
            }
        }

        private bool ObjectCompatWithMode(TileObjectData _object)
        {
            switch (ProcessMode)
            {
                case ProcessMode.Demolish:
                    return _object.Buildable;
                case ProcessMode.Gather:
                    switch (GatherMode)
                    {
                        case GatherMode.Fell:
                            return ((GatherableObjectData)_object).GatherType == GatherMode.Fell;
                        case GatherMode.Mine:
                            return ((GatherableObjectData)_object).GatherType == GatherMode.Mine;
                    }

                    return false;
            }

            return false;
        }

        public void SetZone(Zone _zoneToBuild)
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Zone;
            ZoneToBuild = _zoneToBuild;
        }

        /// <summary>
        ///     Sets the controller to build the provided tile object.
        /// </summary>
        /// <param name="_data"></param>
        public void SetBuildMode(TileObjectData _data)
        {
            MouseController.Instance.Mode = _data.Draggable ? MouseMode.Process : MouseMode.Process_Single;
            ProcessMode = ProcessMode.Object;
            ObjectToBuild = _data;
        }

        /// <summary>
        ///     Sets the controller into demolish mode.
        /// </summary>
        public void SetDemolishMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Demolish;
        }

        public void SetGatherMode(GatherMode _mode)
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Gather;
            GatherMode = _mode;
        }

        public void SetCancelMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Cancel;
        }
    }
}