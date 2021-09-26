using System.Collections.Generic;

namespace ColonyZ.Models.Map.Rendering
{
    public class WorldSection
    {
        public List<WorldSectionLayer> Layers { get; }

        public WorldSection(WorldChunk _chunk)
        {
            Layers = new List<WorldSectionLayer>(_chunk.Tiles.Count);
        }
    }
}