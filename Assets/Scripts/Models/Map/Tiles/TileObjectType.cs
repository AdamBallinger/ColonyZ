namespace Models.Map.Tiles
{
	public enum TileObjectType 
	{
        Single_Tile, // Single tile object such as a chair or something that can only ever occupy 1 tile.
        Multi_Tile // Multi tile object that can be either single, or connect to other tiles to form a larger structure. E.g. Walls, workbench etc.
	}
}
