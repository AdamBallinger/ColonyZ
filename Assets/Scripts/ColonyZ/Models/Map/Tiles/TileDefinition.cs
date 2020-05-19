using UnityEngine;

namespace ColonyZ.Models.Map.Tiles
{
    [CreateAssetMenu(fileName = "Tile_Definition_", menuName = "ColonyZ/Tile Definition", order = 101)]
    public class TileDefinition : ScriptableObject
    {
        [SerializeField, Tooltip("Percentage modifier applied to units walking over this tile.")]
        private float movementModifier;

        [SerializeField, Tooltip("The texture index for this tile in the world mesh texture.")]
        private int textureIndex;

        [SerializeField] private string tileName;

        public string TileName => tileName;

        public int TextureIndex => textureIndex;

        public float MovementModifier => movementModifier;
    }
}