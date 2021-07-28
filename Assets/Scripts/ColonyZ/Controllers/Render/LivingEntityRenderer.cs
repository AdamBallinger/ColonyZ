using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;
using UnityEngine;

namespace ColonyZ.Controllers.Render
{
    public class LivingEntityRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bodyRenderer;

        private LivingEntity entity;

        [SerializeField] private SpriteRenderer headRenderer;

        [SerializeField] private SpriteRenderer itemRenderer;

        public void SetEntity(LivingEntity _entity)
        {
            entity = _entity;
        }

        private void Update()
        {
            if (entity == null) return;

            headRenderer.sprite =
                SpriteCache.GetSprite("Living_Heads", entity.HeadId + (int) entity.Motor.MotorDirection);
            // TODO: Change this when I have sprites for side view bodies.
            bodyRenderer.sprite =
                SpriteCache.GetSprite("Living_Bodies", entity.BodyId + (int) entity.Motor.MotorDirection > 2 ? 1 : 0);
            // TODO: Set item renderer sprite when items and character equipment exists.

            var orderOffset = entity.Position.y < World.Instance.GetTileAt(entity.Position).Y ? 1 : 0;

            headRenderer.sortingOrder = entity.GetSortingOrder() + orderOffset;
            bodyRenderer.sortingOrder = entity.GetSortingOrder() - 1 + orderOffset;
            itemRenderer.sortingOrder = entity.GetSortingOrder() + orderOffset;
        }
    }
}