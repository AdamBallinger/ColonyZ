﻿using Models.Map;
using Models.Map.Structures;
using Models.Map.Tiles;

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
        /// If the build mode is set to structure, this is instance of the structure to build
        /// </summary>
        public TileStructure Structure { get; set; }

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
            if (Structure == null)
            {
                return;
            }

            if (World.Instance.IsStructurePositionValid(Structure, _tile))
            {
                // TODO: Add Job to build structure, rather than this instant build.
                //_tile.InstallStructure(Structure.Clone());
                // Set the tile as a construction base until the job is completed, which should then change the structure.
                _tile.InstallStructure(TileStructureRegistry.GetStructure("Construction_Base"));
            }
        }

        public void StartStructureBuild(string _structureName)
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Structure;
            Structure = TileStructureRegistry.GetStructure(_structureName);
        }

        public void StartDemolishBuild()
        {
            MouseController.Instance.Mode = MouseMode.Build;
            Mode = BuildMode.Demolish;
        }
    }
}
