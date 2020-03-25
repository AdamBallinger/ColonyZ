using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Map.Areas
{
    public class AreaManager
    {
        public static AreaManager Instance { get; private set; }

        public List<Area> areas = new List<Area>();

        public event Action<Area> onAreaCreatedEvent;
        public event Action<Area> onAreaDeletedEvent;

        private AreaManager()
        {
        }

        public static void Create()
        {
            if (Instance == null)
            {
                Instance = new AreaManager();
            }
            else
            {
                Debug.LogWarning("AreaManager already created!");
            }
        }

        public void AddArea(Area _area)
        {
            if (!areas.Contains(_area))
            {
                Debug.Log("Creating a new area.");
                areas.Add(_area);
                onAreaCreatedEvent?.Invoke(_area);
            }
        }

        public void RemoveArea(Area _area)
        {
            if (areas.Contains(_area))
            {
                areas.Remove(_area);
                onAreaDeletedEvent?.Invoke(_area);
            }
        }
    }
}