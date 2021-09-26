using System;
using System.Collections.Generic;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map
{
    public class WorldGridLayout
    {
        public List<WorldChunk> Chunks { get; }

        /// <summary>
        ///     Event called when the chunk is modified (Object placed/removed etc.).
        /// </summary>
        public event Action<WorldChunk> chunkModifiedEvent;

        private const int CHUNK_SIZE = 16;

        private World world;

        public WorldGridLayout(World _world)
        {
            world = _world;
            Chunks = new List<WorldChunk>();
        }

        /// <summary>
        ///     Sets dirty status of the chunk at given tiles position to the given state.
        /// </summary>
        /// <param name="_tile"></param>
        /// <param name="_dirty"></param>
        public void SetDirty(Tile _tile, bool _dirty)
        {
            var source = GetChunkAt(_tile);
            source.SetDirty(_dirty);

            // The bottom left world coordinate of the source chunk.
            var sourceNearX = source.X * CHUNK_SIZE;
            var sourceNearY = source.Y * CHUNK_SIZE;
            // The top right world coordinate of the source chunk.
            var sourceFarX = sourceNearX + CHUNK_SIZE - 1;
            var sourceFarY = sourceNearY + CHUNK_SIZE - 1;

            if (_tile.X == sourceNearX && _tile.Y > sourceNearY && _tile.Y < sourceFarY)
            {
                // West chunk
                GetChunkAt(source.X - 1, source.Y)?.SetDirty(_dirty);
            }
            else if (_tile.X == sourceNearX && _tile.Y == sourceFarY)
            {
                // West and North chunks
                GetChunkAt(source.X - 1, source.Y)?.SetDirty(_dirty);
                GetChunkAt(source.X, source.Y + 1)?.SetDirty(_dirty);
            }
            else if (_tile.X > sourceNearX && _tile.X < sourceFarX && _tile.Y == sourceFarY)
            {
                // North chunk
                GetChunkAt(source.X, source.Y + 1)?.SetDirty(_dirty);
            }
            else if (_tile.X == sourceFarX && _tile.Y == sourceFarY)
            {
                // East and North chunks
                GetChunkAt(source.X + 1, source.Y)?.SetDirty(_dirty);
                GetChunkAt(source.X, source.Y + 1)?.SetDirty(_dirty);
            }
            else if (_tile.X == sourceFarX && _tile.Y < sourceFarY && _tile.Y > sourceNearY)
            {
                // East chunk
                GetChunkAt(source.X + 1, source.Y)?.SetDirty(_dirty);
            }
            else if (_tile.X == sourceFarX && _tile.Y == sourceNearY)
            {
                // East and South chunks
                GetChunkAt(source.X + 1, source.Y)?.SetDirty(_dirty);
                GetChunkAt(source.X, source.Y - 1)?.SetDirty(_dirty);
            }
            else if (_tile.X > sourceNearX && _tile.X < sourceFarX && _tile.Y == sourceNearY)
            {
                // South chunk
                GetChunkAt(source.X, source.Y - 1)?.SetDirty(_dirty);
            }
            else if (_tile.X == sourceNearX && _tile.Y == sourceNearY)
            {
                // West and South chunk
                GetChunkAt(source.X - 1, source.Y)?.SetDirty(_dirty);
                GetChunkAt(source.X, source.Y - 1)?.SetDirty(_dirty);
            }
        }

        /// <summary>
        ///     Returns chunk at given world space position.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        private WorldChunk GetChunkAtWorldPos(int _x, int _y)
        {
            var cX = _x / CHUNK_SIZE;
            var cY = _y / CHUNK_SIZE;
            return Chunks.Find(c => c.X == cX && c.Y == cY);
        }

        /// <summary>
        ///     Returns chunk at given chunk space position.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        private WorldChunk GetChunkAt(int _x, int _y)
        {
            return Chunks.Find(c => c.X == _x && c.Y == _y);
        }

        /// <summary>
        ///     Returns chunk found at center of the world.
        /// </summary>
        /// <returns></returns>
        public WorldChunk GetCenterChunk()
        {
            return GetChunkAtWorldPos(world.Width / 2, world.Height / 2);
        }

        /// <summary>
        ///     Returns chunk at given tiles position.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public WorldChunk GetChunkAt(Tile _tile)
        {
            return GetChunkAtWorldPos(_tile.X, _tile.Y);
        }

        /// <summary>
        ///     Returns if the given tile is inside the central world chunk.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public bool IsInCenterChunk(Tile _tile)
        {
            return GetCenterChunk().Contains(_tile);
        }

        /// <summary>
        ///     Notify the world grid that the given tile has changed, raising the chunk modified event.
        /// </summary>
        /// <param name="_tile"></param>
        public void NotifyChunkUpdate(Tile _tile)
        {
            chunkModifiedEvent?.Invoke(GetChunkAt(_tile));
        }

        /// <summary>
        ///     Rebuilds all chunks marked as dirty.
        /// </summary>
        public void RebuildDirty()
        {
            var shouldNotify = false;
            foreach (var chunk in Chunks)
            {
                if (chunk.IsDirty)
                {
                    shouldNotify = true;
                    RegionManager.Instance.UpdateChunk(chunk);
                    chunk.SetDirty(false);
                }
            }

            if (shouldNotify)
            {
                RegionManager.Instance.NotifyRegionsUpdated();
            }
        }

        /// <summary>
        ///     Builds initial world grid.
        /// </summary>
        public void BuildWorldGrid()
        {
            if (Chunks.Count > 0) return;

            var chunksWidth = world.Width / CHUNK_SIZE;
            var chunksHeight = world.Height / CHUNK_SIZE;

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