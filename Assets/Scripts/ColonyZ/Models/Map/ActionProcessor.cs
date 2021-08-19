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
        Zone_Create,
        Zone_Expand,
        Zone_Shrink,
        Zone_Delete,
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
        /// <param name="_rotation"></param>
        public void Process(IEnumerable<Tile> _tiles, ObjectRotation _rotation = ObjectRotation.Default)
        {
            switch (ProcessMode)
            {
                case ProcessMode.Zone_Create:
                    HandleBuildZone(_tiles);
                    break;
                case ProcessMode.Zone_Expand:
                    HandleExpandZone(_tiles);
                    break;
                case ProcessMode.Zone_Shrink:
                    HandleShrinkZone(_tiles);
                    break;
                case ProcessMode.Zone_Delete:
                    HandleDeleteZone(_tiles);
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

        private void HandleBuildZone(IEnumerable<Tile> _tiles)
        {
            var enumerable = _tiles.ToArray();

            foreach (var tile in enumerable)
            {
                if (!ZoneToBuild.CanPlace(tile))
                    return;
            }
            
            ZoneToBuild.AddTiles(enumerable);

            // Create new template instance for the zone being built.
            ZoneToBuild = (Zone) Activator.CreateInstance(ZoneToBuild.GetType());
        }

        private void HandleExpandZone(IEnumerable<Tile> _tiles)
        {
            var enumerable = _tiles.ToArray();
            
            foreach (var tile in enumerable)
            {
                foreach (var n in tile.DirectNeighbours)
                {
                    if (n.Zone != null && n.Zone == ZoneManager.Instance.CurrentZoneBeingModified)
                    {
                        n.Zone.AddTiles(enumerable);
                        return;
                    }
                }
            }
        }

        private void HandleShrinkZone(IEnumerable<Tile> _tiles)
        {
            foreach (var tile in _tiles)
            {
                tile?.Zone?.RemoveTile(tile);
            }
        }

        private void HandleDeleteZone(IEnumerable<Tile> _tiles)
        {
            foreach (var tile in _tiles)
            {
                ZoneManager.Instance.RemoveZone(tile.Zone);
            }
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

            var jobs = from tile in _tiles
                where tile.HasObject && ObjectCompatWithMode(tile.Object.ObjectData)
                select new HarvestJob(tile);

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

        public void SetZoneCreateMode(Zone _zoneToBuild)
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Zone_Create;
            ZoneToBuild = _zoneToBuild;
        }

        public void SetZoneExpandMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Zone_Expand;
        }

        public void SetZoneShrinkMode()
        {
            MouseController.Instance.Mode = MouseMode.Process;
            ProcessMode = ProcessMode.Zone_Shrink;
        }

        public void SetZoneDeleteMode()
        {
            MouseController.Instance.Mode = MouseMode.Process_Single;
            ProcessMode = ProcessMode.Zone_Delete;
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