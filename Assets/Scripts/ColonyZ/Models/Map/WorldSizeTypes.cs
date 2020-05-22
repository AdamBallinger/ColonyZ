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
            public string Name { get; }
            [field: SerializeField] public int Width { get; set; }
            [field: SerializeField] public int Height { get; set; }

            internal WorldSize(string _name, int _width, int _height)
            {
                Name = _name;
                Width = _width;
                Height = _height;
                WorldSizeTypes.SIZES.Add(this);
            }

            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }

        public static readonly List<WorldSize> SIZES = new List<WorldSize>();

        public static readonly WorldSize SMALL = new WorldSize("Small", 96, 96);
        public static readonly WorldSize MEDIUM = new WorldSize("Medium", 128, 128);
        public static readonly WorldSize LARGE = new WorldSize("Large", 160, 160);
        public static readonly WorldSize HUGE = new WorldSize("Huge", 192, 192);
    }
}