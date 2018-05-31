using Models.Map;

namespace Controllers
{
    public enum BuildMode
    {
        Terrain,
        Area,
        Structure,
        Demolish,
        Harvest
    }

    public class BuildModeController
    {
        public BuildMode Mode { get; private set; }

        /// <summary>
        /// If the build mode is set to structure, this is the name of the structure to build.
        /// </summary>
        public string StructureName { get; set; } = string.Empty;

        public BuildModeController()
        {
            Mode = BuildMode.Structure;
        }

        /// <summary>
        /// Performs a build on a given tile.
        /// TODO: This function in future should create the job based on the build mode, check if placement is valid etc.
        /// </summary>
        /// <param name="_tile"></param>
        public void Build(Tile _tile)
        {
            switch (Mode)
            {
                case BuildMode.Structure:
                    HandleStructureBuild(_tile);
                    break;
                case BuildMode.Demolish:
                    _tile.UninstallStructure();
                    break;
            }
        }

        private void HandleStructureBuild(Tile _tile)
        {
            var structure = GetStructure();

            if (structure == null)
            {
                return;
            }

            if (World.Instance.IsStructurePositionValid(structure, _tile))
            {
                _tile.InstallStructure(structure);
            }
        }

        public void StartStructureBuild(string _structureName)
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Structure;
            StructureName = _structureName;
        }

        public void StartDemolishBuild()
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Demolish;
        }

        /// <summary>
        /// Returns a copy of the tile structure set to be built.
        /// </summary>
        /// <returns></returns>
        private TileStructure GetStructure()
        {
            return TileStructureRegistry.GetStructure(StructureName);
        }
    }
}
