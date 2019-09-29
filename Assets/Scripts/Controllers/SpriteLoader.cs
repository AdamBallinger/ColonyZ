using Models.Sprites;
using UnityEngine;

namespace Controllers
{
    public class SpriteLoader : MonoBehaviour
    {
        [SerializeField]
        private SpriteData[] spriteData;
        
        public void Load()
        {
            foreach(var data in spriteData)
            {
                data.Load();
            }
        }
    }
}
