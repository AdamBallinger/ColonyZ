using ColonyZ.Models.Map.Tiles.Objects;
using UnityEngine;

namespace ColonyZ.Models.Map
{
    public class WorldGenerator
    {
        public static int octaves;
        public static float persistance;
        public static float lacunarity;
        public static float noiseScale;
        public static int seed;

        public static float stoneThreshold;

        private int width;
        private int height;


        public WorldGenerator(int _seed)
        {
            seed = _seed;
            persistance = Mathf.Clamp01(persistance);
            width = World.Instance.Width;
            height = World.Instance.Height;
        }

        public void Generate()
        {
            var noiseMap = GenerateNoiseMap();

            for (var i = 0; i < noiseMap.Length; i++)
            {
                var tile = World.Instance.GetTileAt(i);

                if (noiseMap[i] > stoneThreshold)
                {
                    // TODO: Dont place anything within center chunk.
                    tile.RemoveObject(false);
                    tile.SetObject(TileObjectCache.GetObject("Steel Wall"), false);
                }
            }
        }

        private float[] GenerateNoiseMap()
        {
            var noiseMap = new float[width * height];
            var rand = new System.Random(seed);
            var octaveOffsets = new Vector2[octaves];

            for (var i = 0; i < octaves; i++)
            {
                var offsetX = rand.Next(-100000, 100000);
                var offsetY = rand.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (noiseScale <= 0)
            {
                noiseScale = 0.0001f;
            }

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var amplitude = 1.0f;
                    var frequency = 1.0f;
                    var noiseHeight = 1.0f;

                    for (var i = 0; i < octaves; i++)
                    {
                        var sampleX = (x - halfWidth) / noiseScale * frequency + octaveOffsets[i].x;
                        var sampleY = (y - halfHeight) / noiseScale * frequency + octaveOffsets[i].y;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x * width + y] = noiseHeight;
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    noiseMap[x * width + y] = Mathf.InverseLerp(minNoiseHeight,
                        maxNoiseHeight,
                        noiseMap[x * width + y]);
                }
            }

            return noiseMap;
        }
    }
}