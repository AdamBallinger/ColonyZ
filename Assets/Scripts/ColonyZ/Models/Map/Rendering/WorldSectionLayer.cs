namespace ColonyZ.Models.Map.Rendering
{
    public struct WorldSectionLayer
    {
        public WorldSectionRenderType RenderType { get; }

        public WorldSectionLayer(WorldSectionRenderType _renderType)
        {
            RenderType = _renderType;
        }
    }
}