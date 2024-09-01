using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map.Rendering
{
    public class WorldSection
    {
        public Dictionary<WorldSectionRenderType, List<WorldSectionLayer>> Layers { get; }

        /// <summary>
        ///     Chunk associated with this world section.
        /// </summary>
        private WorldChunk chunk;

        public WorldSection(WorldChunk _chunk)
        {
            Layers = new Dictionary<WorldSectionRenderType, List<WorldSectionLayer>>
            {
                { WorldSectionRenderType.TILE_BASE, new List<WorldSectionLayer>() },
                { WorldSectionRenderType.TILE_SURFACE, new List<WorldSectionLayer>() },
                { WorldSectionRenderType.TILE_OBJECT_SMART, new List<WorldSectionLayer>() },
                { WorldSectionRenderType.TILE_OBJECT_STATIC, new List<WorldSectionLayer>() }
            };

            chunk = _chunk;

            World.Instance.WorldGrid.chunkModifiedEvent += _worldChunk =>
            {
                if (!_worldChunk.Equals(chunk)) return;
                
                BuildSectionLayers();
            };
        }

        private void BuildSectionLayers()
        {
            Debug.Log("Building section layers.");
            foreach (var layer in Layers)
            {
                layer.Value.Clear();
            }
            
            var baseLayer = new WorldSectionLayer(WorldSectionRenderType.TILE_BASE);
            foreach (var tile in chunk.Tiles)
            {
                
            }

            Layers[WorldSectionRenderType.TILE_BASE].Add(baseLayer);
        }
    }
}