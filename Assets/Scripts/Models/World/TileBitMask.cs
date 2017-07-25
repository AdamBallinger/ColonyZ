using System;
using System.Collections.Generic;

namespace Models.World
{
    [Flags]
    public enum TileCardinals
    {
        North_West = 1 << 1,
        North = 1 << 2,
        North_East = 1 << 4,
        West = 1 << 8,
        East = 1 << 16,
        South_West = 1 << 32,
        South = 1 << 64,
        South_East = 2 << 128
    }

    public class TileBitMask
    {

        public static readonly Dictionary<int, int> bitMaskMap = new Dictionary<int, int>
        {
            {2, 1},
            {8, 2},
            {10, 3},
            {11, 4},
            {16, 5},
            {18, 6},
            {22, 7},
            {24, 8},
            {26, 9},
            {27, 10},
            {30, 11},
            {31, 12},
            {64, 13},
            {66, 14},
            {72, 15},
            {74, 16},
            {75, 17},
            {80, 18},
            {82, 19},
            {86, 20},
            {88, 21},
            {90, 22},
            {91, 23},
            {94, 24},
            {95, 25},
            {104, 26},
            {106, 27},
            {107, 28},
            {120, 29},
            {122, 30},
            {123, 31},
            {126, 32},
            {127, 33},
            {208, 34},
            {210, 35},
            {214, 36},
            {216, 37},
            {218, 38},
            {219, 39},
            {222, 40},
            {223, 41},
            {248, 42},
            {250, 43},
            {251, 44},
            {254, 45},
            {255, 46},
            {0, 47}
        };

        public static int ComputeBitmaskValue(Tile _origin)
        {
            var bitmaskValue = 0;

            var tile_NW = World.Instance.GetTileAt(_origin.X - 1, _origin.Y + 1);
            var tile_N = World.Instance.GetTileAt(_origin.X, _origin.Y + 1);
            var tile_NE = World.Instance.GetTileAt(_origin.X + 1, _origin.Y + 1);
            var tile_W = World.Instance.GetTileAt(_origin.X - 1, _origin.Y);
            var tile_E = World.Instance.GetTileAt(_origin.X + 1, _origin.Y);
            var tile_SW = World.Instance.GetTileAt(_origin.X - 1, _origin.Y - 1);
            var tile_S = World.Instance.GetTileAt(_origin.X, _origin.Y - 1);
            var tile_SE = World.Instance.GetTileAt(_origin.X + 1, _origin.Y - 1);

            if(tile_NW?.InstalledStructure != null && tile_N?.InstalledStructure != null && tile_W?.InstalledStructure != null)
            {
                bitmaskValue += 1;
            }
            
            if(tile_N?.InstalledStructure != null)
            {
                bitmaskValue += 2;
            }

            if(tile_NE?.InstalledStructure != null && tile_N?.InstalledStructure != null && tile_E?.InstalledStructure != null)
            {
                bitmaskValue += 4;
            }

            if(tile_W?.InstalledStructure != null)
            {
                bitmaskValue += 8;
            }

            if(tile_E?.InstalledStructure != null)
            {
                bitmaskValue += 16;
            }

            if(tile_SW?.InstalledStructure != null && tile_S?.InstalledStructure != null && tile_W?.InstalledStructure != null)
            {
                bitmaskValue += 32;
            }

            if(tile_S?.InstalledStructure != null)
            {
                bitmaskValue += 64;
            }

            if(tile_SE?.InstalledStructure != null && tile_S?.InstalledStructure != null && tile_E?.InstalledStructure != null)
            {
                bitmaskValue += 128;
            }

            return bitmaskValue;
        }
    }
}
