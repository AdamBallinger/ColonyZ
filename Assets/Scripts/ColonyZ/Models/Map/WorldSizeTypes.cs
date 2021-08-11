using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map
{
    public static class WorldSizeTypes
    {
        [Serializable]
        public struct WorldSize
        {
            [field: SerializeField] public string Name { get; private set; }
            [field: SerializeField] public int Width { get; set; }
            [field: SerializeField] public int Height { get; set; }

            public bool Available { get; }

            internal WorldSize(string _name, int _width, int _height, bool _available = true)
            {
                Name = _name;
                Width = _width;
                Height = _height;
                Available = _available;
                SIZES.Add(this);
            }

            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }

        public static readonly List<WorldSize> SIZES = new List<WorldSize>();

        public static readonly WorldSize MINI = new WorldSize("Mini", 16, 16, false);
        public static readonly WorldSize TINY = new WorldSize("Tiny", 48, 48, false);
        public static readonly WorldSize SMALL = new WorldSize("Small", 128, 128);
        public static readonly WorldSize MEDIUM = new WorldSize("Medium", 192, 192);
        public static readonly WorldSize LARGE = new WorldSize("Large", 256, 256);
        public static readonly WorldSize HUGE = new WorldSize("Huge", 320, 320);
    }
}