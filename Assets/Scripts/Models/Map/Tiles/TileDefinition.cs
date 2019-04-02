using System;
using Newtonsoft.Json;

namespace Models.Map.Tiles
{
    [Serializable]
    public class TileDefinition
    {        
        /// <summary>
        /// The texture index for this definition.
        /// </summary>
        [JsonProperty]
        public int TextureIndex { get; private set; }
        
        /// <summary>
        /// The name of the tile.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }
        
        /// <summary>
        /// Movement modifier for characters moving over this tile.
        /// A higher value increases movement speed; a lower value reduces.
        /// </summary>
        [JsonProperty]
        public float MovementModifier { get; set; }
    }
}