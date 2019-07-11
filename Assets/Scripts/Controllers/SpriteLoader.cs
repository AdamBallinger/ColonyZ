using Models.Sprites;
using UnityEngine;

namespace Controllers
{
    public class SpriteLoader : MonoBehaviour
    {
        [SerializeField]
        private SpriteData[] spriteData;
        
        private void Awake()
        {
            foreach(var data in spriteData)
            {
                data.Load();
            }
        }
    }
}
