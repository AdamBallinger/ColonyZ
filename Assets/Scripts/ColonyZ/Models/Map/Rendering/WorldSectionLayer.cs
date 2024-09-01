using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Rendering
{
    public struct WorldSectionLayer
    {
        public WorldSectionRenderType RenderType { get; }

        private List<TileObjectData> objects;

        public WorldSectionLayer(WorldSectionRenderType _renderType)
        {
            RenderType = _renderType;

            objects = new List<TileObjectData>();
        }
    }
}