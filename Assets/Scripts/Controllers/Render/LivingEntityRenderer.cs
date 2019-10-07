using Models.Entities.Living;
using Models.Sprites;
using UnityEngine;

namespace Controllers.Render
{
    public class LivingEntityRenderer : MonoBehaviour
    {
        private LivingEntity entity;

        [SerializeField]
        private SpriteRenderer headRenderer;
        
        [SerializeField]
        private SpriteRenderer bodyRenderer;
        
        [SerializeField]
        private SpriteRenderer itemRenderer;
        
        public void SetEntity(LivingEntity _entity)
        {
            entity = _entity;
        }
        
        private void Update()
        {
            if (entity == null)
            {
                return;
            }

            headRenderer.sprite = SpriteCache.GetSprite("Living_Heads", entity.HeadId + entity.Motor.DirectionIndex);
            // TODO: Change this when I have sprites for side view bodies.
            bodyRenderer.sprite = SpriteCache.GetSprite("Living_Bodies", entity.BodyId + entity.Motor.DirectionIndex > 2 ? 1 : 0);
            // TODO: Set item renderer sprite when items and character equipment exists.

            headRenderer.sortingOrder = entity.GetSortingOrder();
            bodyRenderer.sortingOrder = entity.GetSortingOrder() - 1;
            itemRenderer.sortingOrder = entity.GetSortingOrder();
        }
    }
}