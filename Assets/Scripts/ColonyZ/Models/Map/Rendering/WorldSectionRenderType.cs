namespace ColonyZ.Models.Map.Rendering
{
    public enum WorldSectionRenderType
    {
        //  Use for drawing the ground texture of the world.
        TILE_BASE = 0,
        //  Use for drawing "flooring" type textures.
        TILE_SURFACE = 1,
        //  Use for drawing objects that appearance change based on surrounding objects (Walls).
        TILE_OBJECT_SMART = 2,
        //  Use for drawing static objects.
        TILE_OBJECT_STATIC = 3 
    }
}