using UnityEngine;

namespace Models.Map
{
    public static class WorldGenerator
    {
        public static void Generate()
        {
            for(var x = 0; x < World.Instance.Width; x++)
            {
                for(var y = 0; y < World.Instance.Height; y++)
                {
                    if(Random.Range(0.0f, 1.0f) >= 0.92f)
                    {
                        World.Instance.SetTileAt(x, y, "Mud", TileType.Mud);
                    }
                }
            }
        }
    }
}