using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Map.Zones
{
    public class ZoneManager
    {
        public static ZoneManager Instance { get; private set; }

        private readonly List<Zone> zones = new List<Zone>();

        public event Action<Zone> zoneCreatedEvent;
        public event Action<Zone> zoneDeletedEvent;

        private ZoneManager()
        {
        }

        public static void Create()
        {
            if (Instance == null)
            {
                Instance = new ZoneManager();
            }
            else
            {
                Debug.LogWarning("ZoneManager already created!");
            }
        }

        public void AddZone(Zone _zone)
        {
            if (!zones.Contains(_zone))
            {
                zones.Add(_zone);
                zoneCreatedEvent?.Invoke(_zone);
            }
        }

        public void RemoveZone(Zone _zone)
        {
            if (zones.Contains(_zone))
            {
                zones.Remove(_zone);
                zoneDeletedEvent?.Invoke(_zone);
            }
        }
    }
}