﻿using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Controllers;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;

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
        public TileObject ObjectToBuild { get; private set; }

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
        public void Process(IEnumerable<Tile> _tiles, int _x = 0, int _y = 0, int _width = 0, int _height = 0)
        {
            switch (ProcessMode)
            {
                case ProcessMode.Zone:
                    if (ZoneToBuild.MinimumSize.x <= _width && ZoneToBuild.MinimumSize.y <= _height)
                        HandleBuildZone(_tiles, _x, _y, _width, _height);
                    break;
                case ProcessMode.Object:
                    HandleBuild(_tiles);
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
                {
                    if (tile.HasObject && ObjectCompatWithMode(tile.Object))
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
                where tile.HasObject && ObjectCompatWithMode(tile.Object)
                select new DemolishJob(tile));

            JobManager.Instance.AddJobs(jobs);
        }

        private void HandleGather(IEnumerable<Tile> _tiles)
        {
            if (GodMode)
            {
                foreach (var tile in _tiles)
                {
                    if (tile.HasObject && ObjectCompatWithMode(tile.Object))
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
                where tile.HasObject && ObjectCompatWithMode(tile.Object)
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

        private bool ObjectCompatWithMode(TileObject _object)
        {
            switch (ProcessMode)
            {
                case ProcessMode.Demolish:
                    return _object.Buildable;
                case ProcessMode.Gather:
                    switch (GatherMode)
                    {
                        case GatherMode.Fell:
                            return _object.Fellable;
                        case GatherMode.Mine:
                            return _object.Mineable;
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
        /// <param name="_object"></param>
        public void SetBuildMode(TileObject _object)
        {
            MouseController.Instance.Mode = _object.Draggable ? MouseMode.Process : MouseMode.Process_Single;
            ProcessMode = ProcessMode.Object;
            ObjectToBuild = _object;
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