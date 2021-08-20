using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColonyZ.Models.Map.Zones
{
    public class ZoneManager
    {
        public static ZoneManager Instance { get; private set; }

        public List<Zone> Zones { get; } = new List<Zone>();

        public event Action<Zone> zoneCreatedEvent;
        public event Action<Zone> zoneUpdatedEvent;
        public event Action<Zone> zoneDeletedEvent;

        public Zone CurrentZoneBeingModified { get; set; }

        private ZoneManager()
        {
        }

        public static void Create()
        {
            if (Instance == null)
                Instance = new ZoneManager();
            else
                Debug.LogWarning("ZoneManager already created!");
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public int GetZoneTypeQuantity<T>() where T : Zone
        {
            return Zones.OfType<T>().Count();
        }

        public void OnZonePlaced(Zone _zone)
        {
            if (CurrentZoneBeingModified != null)
            {
                foreach (var tile in _zone.Tiles)
                {
                    foreach (var n in tile.DirectNeighbours)
                    {
                        if (n.Zone == CurrentZoneBeingModified)
                        {
                            CurrentZoneBeingModified.AddTiles(_zone.Tiles, false);
                            zoneUpdatedEvent?.Invoke(CurrentZoneBeingModified);
                            return;
                        }
                    }
                }
                
                CurrentZoneBeingModified = _zone;
            }

            AddZone(_zone);
            if (CurrentZoneBeingModified == null)
            {
                CurrentZoneBeingModified = _zone;
            }
        }

        public void OnZoneRemoved(Zone _zone)
        {
            if (_zone.Size == 0)
            {
                RemoveZone(_zone);
                return;
            }
            
            zoneUpdatedEvent?.Invoke(_zone);
        }
        
        private void AddZone(Zone _zone)
        {
            if (!Zones.Contains(_zone))
            {
                Zones.Add(_zone);
                zoneCreatedEvent?.Invoke(_zone);
            }
        }

        public void RemoveZone(Zone _zone)
        {
            if (Zones.Contains(_zone))
            {
                Zones.Remove(_zone);
                _zone.RemoveAllTiles();

                zoneDeletedEvent?.Invoke(_zone);
            }
        }
    }
}