using Models.AI;
using Models.Map.Tiles;
using Models.Sprites;
using UnityEngine;

namespace Models.Entities.Living
{
    public abstract class LivingEntity : Entity
    {
        public float MovementSpeed { get; set; }
        
        public AIMotor Motor { get; }
        
        public int HeadId { get; protected set; }
        
        public int BodyId { get; protected set; }
        
        protected int Health { get; set; }

        protected LivingEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 1.0f;
            Health = 100;
            Motor = new AIMotor(this);
        }

        public override void Update()
        {
            Motor.Update();
        }

        /// <summary>
        /// Event called when the ai motor fails to find a path to its provided target tile.
        /// </summary>
        public virtual void OnPathFailed() {}

        public override Sprite GetSelectionIcon()
        {
            // Each head has 4 sprites so multiply the head id by 4 to get the facing sprite for the entity head id.
            return SpriteCache.GetSprite("Living_Heads", HeadId * 4);
        }

        public override string GetSelectionDescription()
        {
            return base.GetSelectionDescription() +
                   $"Health: {Health}\n";
        }
    }
}
