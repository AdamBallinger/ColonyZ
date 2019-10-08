using UnityEngine;

namespace Models.Items
{
    [CreateAssetMenu(fileName = "Item_Resource_", menuName = "ColonyZ/Resource Item", order = 81)]
    public class ResourceItem : Item
    {
        [SerializeField]
        private bool expires;

        [SerializeField]
        private bool requiresCoolStorage;
    }
}