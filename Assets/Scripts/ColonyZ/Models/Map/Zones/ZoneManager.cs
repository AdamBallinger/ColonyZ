using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map.Zones
{
    public class ZoneManager
    {
        public static ZoneManager Instance { get; private set; }

        public List<Zone> Zones { get; } = new List<Zone>();

        public event Action<Zone> zoneCreatedEvent;
        public event Action<Zone> zoneDeletedEvent;

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

        public void AddZone(Zone _zone)
        {
            if (!Zones.Contains(_zone))
            {
                Zones.Add(_zone);
                for (var x = _zone.Origin.x; x <= _zone.Origin.x + _zone.Size.x; x++)
                {
                    for (var y = _zone.Origin.y; y <= _zone.Origin.y + _zone.Size.y; y++)
                    {
                        World.Instance.GetTileAt(x, y).Zone = _zone;
                    }
                }

                zoneCreatedEvent?.Invoke(_zone);
            }
        }

        public void RemoveZone(Zone _zone)
        {
            if (Zones.Contains(_zone))
            {
                Zones.Remove(_zone);
                zoneDeletedEvent?.Invoke(_zone);
            }
        }
    }
}