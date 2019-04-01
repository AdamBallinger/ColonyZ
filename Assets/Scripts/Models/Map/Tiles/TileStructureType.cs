namespace Models.Map.Tiles
{
	public enum TileStructureType 
	{
        Single_Tile, // Single tile structure such as a chair or something that can only ever occupy 1 tile.
        Multi_Tile // Multi tile structure that can be either single, or connect to other tiles to form a larger structure. E.g. Walls, workbench etc.
	}
}
