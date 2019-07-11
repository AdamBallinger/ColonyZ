using Controllers;
using UnityEngine;

namespace Models.Sprites
{
    [CreateAssetMenu(fileName = "Sprite_Data_", menuName = "ColonyZ/Sprite Data")]
    public class SpriteData : ScriptableObject
    {
        public Sprite[] Sprites => sprites;

        public int SpriteCount => Sprites.Length;

        public string SpriteGroup => spriteGroupName;

        public int IconIndex => Sprites.Length > 1 ? uiIconIndex : 0;

        [SerializeField, Tooltip("The group the sprites will be stored in for the sprite cache.")]
        private string spriteGroupName = "default";

        [SerializeField]
        private Sprite[] sprites;

        [SerializeField, HideInInspector]
        private int uiIconIndex;
        
        public void Load()
        {
            // TODO: Add sprite loading once for each scriptable object in resources.
            SpriteCache.AddSprites(SpriteGroup, Sprites);
        }
    }
}