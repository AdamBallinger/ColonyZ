using System.Collections.Generic;

namespace Models.Map
{
    public class WorldGridLayout
    {
        public List<WorldChunk> Chunks { get; }

        private World world;

        private const int GRID_SIZE = 16;

        public WorldGridLayout(World _world)
        {
            world = _world;
            Chunks = new List<WorldChunk>();
            BuildWorldGrid();
        }

        public WorldChunk GetChunkAt(int _x, int _y)
        {
            var cX = _x / GRID_SIZE;
            var cY = _y / GRID_SIZE;
            return Chunks.Find(c => c.X == cX && c.Y == cY);
        }

        private void BuildWorldGrid()
        {
            for (var cx = 0; cx < world.Width / GRID_SIZE; cx++)
            {
                for (var cy = 0; cy < world.Height / GRID_SIZE; cy++)
                {
                    var chunk = new WorldChunk(cx, cy);
                    for (var x = 0; x < GRID_SIZE; x++)
                    {
                        for (var y = 0; y < GRID_SIZE; y++)
                        {
                            var xOffset = cx * 16;
                            var yOffset = cy * 16;
                            chunk.Add(world.GetTileAt(x + xOffset, y + yOffset));
                        }
                    }

                    Chunks.Add(chunk);
                }
            }
        }
    }
}