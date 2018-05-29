namespace Models.Map.Structures
{
    public class WoodWallStructure : TileStructure
    {
        public WoodWallStructure(string _structureName) : base(_structureName)
        {
            Type = TileStructureType.Multi_Tile;
        }
    }
}
