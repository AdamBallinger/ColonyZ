﻿using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Wall_", menuName = "ColonyZ/Wall Object", order = 52)]
    public class WallObject : TileObject
    {
        public override bool CanPlace(Tile _tile)
        {
            return _tile.Object == null;
        }
    }
}
