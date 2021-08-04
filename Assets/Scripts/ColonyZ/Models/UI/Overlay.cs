using System;
using System.Collections.Generic;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using UnityEngine;

namespace ColonyZ.Models.UI
{
    public class Overlay
    {
        public byte[] OverlayArray { get; }
        public float[] OverlayAlpha { get; }

        public event Action overlayUpdatedEvent;
        
        private World world;

        public Overlay(World _world)
        {
            world = _world;
            OverlayArray = new byte[world.Width * world.Height];
            OverlayAlpha = new float[world.Width * world.Height];
        }

        public void Init()
        {
            JobManager.Instance.jobCreatedEvent += j =>
            {
                if (j is HarvestJob job) SetOverlayAtTile(j.TargetTile, GetOverlayForHarvestJob(job));
                else if (j is DemolishJob) SetOverlayAtTile(j.TargetTile, OverlayType.Hammer);
            };
            
            JobManager.Instance.jobStateChangedEvent += j =>
            {
                if (j.State == JobState.Error) SetOverlayAtTile(j.TargetTile, OverlayType.Red_Cross, 1.0f);
                else if (j is DemolishJob) SetOverlayAtTile(j.TargetTile, OverlayType.Hammer);
                else if (j is HarvestJob job) SetOverlayAtTile(j.TargetTile, GetOverlayForHarvestJob(job));
                else ClearOverlayAtTile(j.TargetTile);
            };

            JobManager.Instance.jobCompletedEvent += j => ClearOverlayAtTile(j.TargetTile);
        }

        public void SetOverlayAtTile(Tile _tile, OverlayType _overlayType, float _alpha = 0.5f)
        {
            if (_tile == null) return;
            var index = world.GetTileIndex(_tile);
            OverlayArray[index] = (byte)_overlayType;
            OverlayAlpha[index] = _alpha;
            overlayUpdatedEvent?.Invoke();
        }

        public void SetOverlayForTiles(List<Tile> _tiles, OverlayType _overlayType, float _alpha = 0.5f)
        {
            for (var i = 0; i < _tiles.Count; i++)
            {
                var index = world.GetTileIndex(_tiles[i]);
                OverlayArray[index] = (byte)_overlayType;
                OverlayAlpha[index] = _alpha;
            }
            
            overlayUpdatedEvent?.Invoke();
        }

        public void SetOverlayAtPosition(Vector2 _position, OverlayType _overlayType, float _alpha = 0.5f)
        {
            SetOverlayAtTile(world.GetTileAt(_position), _overlayType, _alpha);
        }

        public void ClearOverlayAtTile(Tile _tile)
        {
            SetOverlayAtTile(_tile, OverlayType.None);
        }

        public void ClearOverlayAtTiles(List<Tile> _tiles)
        {
            SetOverlayForTiles(_tiles, OverlayType.None);
        }

        private OverlayType GetOverlayForHarvestJob(HarvestJob _job)
        {
            if (!_job.TargetTile.HasObject) return OverlayType.None;
            
            if (_job.TargetTile.HasObject)
            {
                if (_job.TargetTile.Object.Mineable) return OverlayType.Pickaxe;
                if (_job.TargetTile.Object.Fellable) return OverlayType.Axe;
            }

            return OverlayType.None;
        }
    }
}