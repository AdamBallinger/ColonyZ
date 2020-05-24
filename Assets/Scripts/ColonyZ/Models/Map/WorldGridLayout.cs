using System;
using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map
{
    public class WorldGridLayout
    {
        public List<WorldChunk> Chunks { get; }

        public event Action<WorldChunk> chunkUpdateEvent;

        private const int CHUNK_SIZE = 12;

        private World world;

        public WorldGridLayout(World _world)
        {
            world = _world;
            Chunks = new List<WorldChunk>();
        }

        private WorldChunk GetChunkAt(int _x, int _y)
        {
            var cX = _x / CHUNK_SIZE;
            var cY = _y / CHUNK_SIZE;
            return Chunks.Find(c => c.X == cX && c.Y == cY);
        }

        public WorldChunk GetChunkAt(Tile _tile)
        {
            return GetChunkAt(_tile.X, _tile.Y);
        }

        public List<WorldChunk> GetChunkNeighbours(WorldChunk _chunk)
        {
            var chunks = new List<WorldChunk>();

            foreach (var chunk in Chunks)
            {
                var cdx = Math.Abs(_chunk.X - chunk.X);
                var cdy = Math.Abs(_chunk.Y - chunk.Y);
                if (cdx == 1 && cdy == 0 || cdx == 0 && cdy == 1)
                {
                    chunks.Add(chunk);
                }
            }

            return chunks;
        }

        public void NotifyChunkUpdate(Tile _tile)
        {
            chunkUpdateEvent?.Invoke(GetChunkAt(_tile));
        }

        public void BuildWorldGrid()
        {
            if (Chunks.Count > 0) return;

            var chunksWidth = world.Width / CHUNK_SIZE;
            chunksWidth += world.Width / chunksWidth;

            var chunksHeight = world.Height / CHUNK_SIZE;
            chunksHeight += world.Height / chunksHeight;

            for (var cy = 0; cy < chunksHeight; cy++)
            {
                for (var cx = 0; cx < chunksWidth; cx++)
                {
                    var chunk = new WorldChunk(cx, cy);
                    for (var y = 0; y < CHUNK_SIZE; y++)
                    {
                        for (var x = 0; x < CHUNK_SIZE; x++)
                        {
                            var xOffset = cx * CHUNK_SIZE;
                            var yOffset = cy * CHUNK_SIZE;
                            var tile = world.GetTileAt(x + xOffset, y + yOffset);
                            if (tile != null)
                            {
                                chunk.Add(tile);
                            }
                        }
                    }

                    Chunks.Add(chunk);
                }
            }
        }
    }
}