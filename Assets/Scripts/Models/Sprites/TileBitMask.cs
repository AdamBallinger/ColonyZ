using System;
using System.Collections.Generic;
using Models.Map;

namespace Models.Sprites
{
    [Flags]
    public enum Cardinals
    {
        None = 0,
        North_West = 1 << 0,
        North = 1 << 1,
        North_East = 1 << 2,
        West = 1 << 3,
        East = 1 << 4,
        South_West = 1 << 5,
        South = 1 << 6,
        South_East = 1 << 7
    }

    public enum BitmaskEvaluationType
    {
        Tile_Type,
        Tile_Structure
    }

    public class TileBitMask
    {
        /// <summary>
        /// Bit masking dictionary that maps different bitmask values to the matching index of a tileset
        /// [bitMaskIndex, tileSetIndex]
        /// </summary>
        private static readonly Dictionary<int, int> bitMaskMap = new Dictionary<int, int>
        {
            {2, 1}, {8, 2}, {10, 3}, {11, 4}, {16, 5}, {18, 6}, {22, 7}, {24, 8},
            {26, 9}, {27, 10}, {30, 11}, {31, 12},  {64, 13}, {66, 14},  {72, 15}, {74, 16},
            {75, 17}, {80, 18}, {82, 19}, {86, 20}, {88, 21}, {90, 22}, {91, 23}, {94, 24},
            {95, 25}, {104, 26}, {106, 27}, {107, 28}, {120, 29}, {122, 30}, {123, 31}, {126, 32},
            {127, 33}, {208, 34}, {210, 35}, {214, 36}, {216, 37}, {218, 38}, {219, 39}, {222, 40},
            {223, 41}, {248, 42}, {250, 43}, {251, 44}, {254, 45}, {255, 46}, {0, 47}
        };

        private static Tile Origin;
        private static BitmaskEvaluationType Type;

        /// <summary>
        /// Returns the bitmask tileset value for a given tile based on its surrounding tiles according to the evaluation type.
        /// </summary>
        /// <param name="_origin"></param>
        /// <param name="_evalType"></param>
        /// <returns></returns>
        public static int ComputeBitmaskValue(Tile _origin, BitmaskEvaluationType _evalType)
        {
            var bitmaskValue = 0;

            Origin = _origin;
            Type = _evalType;

            var tile_NW = World.Instance.GetTileAt(_origin.X - 1, _origin.Y + 1);
            var tile_N = World.Instance.GetTileAt(_origin.X, _origin.Y + 1);
            var tile_NE = World.Instance.GetTileAt(_origin.X + 1, _origin.Y + 1);
            var tile_W = World.Instance.GetTileAt(_origin.X - 1, _origin.Y);
            var tile_E = World.Instance.GetTileAt(_origin.X + 1, _origin.Y);
            var tile_SW = World.Instance.GetTileAt(_origin.X - 1, _origin.Y - 1);
            var tile_S = World.Instance.GetTileAt(_origin.X, _origin.Y - 1);
            var tile_SE = World.Instance.GetTileAt(_origin.X + 1, _origin.Y - 1);

            if (Connects(tile_NW) && Connects(tile_N) && Connects(tile_W)/*tile_NW?.Structure != null && tile_N?.Structure != null && tile_W?.Structure != null*/)
            {
                bitmaskValue += 1;
            }

            if (Connects(tile_N)/*tile_N?.Structure != null*/)
            {
                bitmaskValue += 2;
            }

            if (Connects(tile_NE) && Connects(tile_N) && Connects(tile_E)/*tile_NE?.Structure != null && tile_N?.Structure != null && tile_E?.Structure != null*/)
            {
                bitmaskValue += 4;
            }

            if (Connects(tile_W)/*tile_W?.Structure != null*/)
            {
                bitmaskValue += 8;
            }

            if (Connects(tile_E)/*tile_E?.Structure != null*/)
            {
                bitmaskValue += 16;
            }

            if (Connects(tile_SW) && Connects(tile_S) && Connects(tile_W)/*tile_SW?.Structure != null && tile_S?.Structure != null && tile_W?.Structure != null*/)
            {
                bitmaskValue += 32;
            }

            if (Connects(tile_S)/*tile_S?.Structure != null*/)
            {
                bitmaskValue += 64;
            }

            if (Connects(tile_SE) && Connects(tile_S) && Connects(tile_E)/*tile_SE?.Structure != null && tile_S?.Structure != null && tile_E?.Structure != null*/)
            {
                bitmaskValue += 128;
            }

            return bitMaskMap[bitmaskValue];
        }

        private static bool Connects(Tile _neighbour)
        {
            if (Type == BitmaskEvaluationType.Tile_Structure)
            {
                if (Origin?.Structure == null || _neighbour?.Structure == null)
                {
                    return false;
                }

                return Origin.Structure.ConnectsWith(_neighbour.Structure);
            }

            return false;
        }
    }
}
