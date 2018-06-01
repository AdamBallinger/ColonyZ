using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models.Sprites
{
    [Serializable]
    public enum SpriteType
    {
        Single,
        Tileset
    }

    [Serializable]
    public class SpriteData
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public SpriteType SpriteType { get; private set; } = SpriteType.Single;

        [JsonProperty]
        public string ResourcePath { get; private set; }

        [JsonProperty]
        public string SpriteGroup { get; private set; } = "default";

        [JsonProperty]
        public string SpriteGroupPrefix { get; private set; } = "";
    }
}
