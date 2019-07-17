using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Foundation_", menuName = "ColonyZ/Foundation Object", order = 52)]
    public class FoundationObject : TileObject
    {
        /// <summary>
        /// The TileObject that will be built at this foundation.
        /// </summary>
        public TileObject Building { get; private set; }
        
        /// <summary>
        /// Building progress for this foundation. Completed when value is 1.
        /// </summary>
        public float Progress { get; set; }
        
        public override bool CanPlace(Tile _tile)
        {
            return _tile?.Object == null;
        }

        public override void Update()
        {
            Progress += 0.6f * Time.deltaTime;
            
            if (Progress >= 1.0f)
            {
                Tile.RemoveObject();
                Tile.SetObject(Building);
            }
        }
        
        public void SetBuilding(TileObject _object)
        {
            Building = _object;
            Building.OriginTile = OriginTile;
            Building.Tile = Tile;
        }

        public override bool ConnectsWith(TileObject _other)
        {
            if (_other.GetType() != typeof(FoundationObject))
            {
                return false;
            }

            // Only connect with other foundations that are building the same object.
            var foundation = _other as FoundationObject;
            return foundation != null && string.CompareOrdinal(foundation.Building?.ObjectName, Building?.ObjectName) == 0;
        }
    }
}