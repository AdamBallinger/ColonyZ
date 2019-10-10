using Models.Items;
using UnityEngine;

namespace Controllers.Loaders
{
    public class ItemLoader : MonoBehaviour
    {
        [SerializeField]
        private Item[] items;
        
        public void Load()
        {
            foreach (var item in items)
            {
                ItemManager.RegisterItem(item);
            }
        }
    }
}