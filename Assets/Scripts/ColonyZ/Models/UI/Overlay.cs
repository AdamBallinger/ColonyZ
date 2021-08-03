using System;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using UnityEngine;

namespace ColonyZ.Models.UI
{
    public class Overlay
    {
        public byte[] OverlayArray { get; }

        public event Action onOverlayUpdated;
        
        private World world;

        public Overlay(World _world)
        {
            world = _world;
            OverlayArray = new byte[world.Width * world.Height];
        }

        public void Init()
        {
            JobManager.Instance.jobStateChangedEvent += job =>
            {
                if (job.State == JobState.Error) SetOverlayAtTile(job.TargetTile, OverlayType.Red_Cross);
                else ClearOverlayAtTile(job.TargetTile);
            };
        }

        public void SetOverlayAtTile(Tile _tile, OverlayType _overlayType)
        {
            OverlayArray[world.GetTileIndex(_tile)] = (byte)_overlayType;
            onOverlayUpdated?.Invoke();
        }

        public void SetOverlayAtPosition(Vector2 _position, OverlayType _overlayType)
        {
            SetOverlayAtTile(world.GetTileAt(_position), _overlayType);
        }

        public void ClearOverlayAtTile(Tile _tile)
        {
            SetOverlayAtTile(_tile, OverlayType.None);
        }
    }
}