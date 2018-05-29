namespace Models.Map.Structures
{
    public class SteelWallStructure : TileStructure
    {
        public SteelWallStructure(string _structureName) : base(_structureName)
        {
            Type = TileStructureType.Multi_Tile;
        }
    }
}
