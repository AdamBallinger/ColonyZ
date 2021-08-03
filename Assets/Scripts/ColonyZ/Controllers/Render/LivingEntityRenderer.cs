using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;
using UnityEngine;

namespace ColonyZ.Controllers.Render
{
    public class LivingEntityRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bodyRenderer;

        [SerializeField] private SpriteRenderer headRenderer;

        [SerializeField] private SpriteRenderer itemRenderer;
        
        private LivingEntity entity;

        public void SetEntity(LivingEntity _entity)
        {
            entity = _entity;
        }

        private void Update()
        {
            if (entity == null) return;

            headRenderer.sprite =
                SpriteCache.GetSprite("Living_Heads", entity.HeadId * 4 + (int) entity.Motor.MotorDirection);
            bodyRenderer.sprite =
                SpriteCache.GetSprite("Living_Bodies", entity.BodyId * 4 + (int) entity.Motor.MotorDirection);
            // TODO: Set item renderer sprite when items and character equipment exists.

            var orderOffset = entity.Position.y < World.Instance.GetTileAt(entity.Position).Y ? 1 : 0;

            headRenderer.sortingOrder = entity.GetSortingOrder() + orderOffset;
            bodyRenderer.sortingOrder = entity.GetSortingOrder() - 1 + orderOffset;
            // TODO: Sort item behind body if moving upwards.
            itemRenderer.sortingOrder = entity.GetSortingOrder() + orderOffset;
        }
    }
}