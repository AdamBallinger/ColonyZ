using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map
{
    [Serializable]
    public struct WorldSize
    {
        [field: SerializeField] public int Width { get; set; }
        [field: SerializeField] public int Height { get; set; }

        public WorldSize(int _width, int _height)
        {
            Width = _width;
            Height = _height;
            WorldSizeTypes.SIZES.Add(this);
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }

    [Serializable]
    public static class WorldSizeTypes
    {
        public static readonly List<WorldSize> SIZES = new List<WorldSize>();

        public static readonly WorldSize SMALL = new WorldSize(76, 76);
        public static readonly WorldSize MEDIUM = new WorldSize(112, 112);
        public static readonly WorldSize LARGE = new WorldSize(156, 156);
    }
}