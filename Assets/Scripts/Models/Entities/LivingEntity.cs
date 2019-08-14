using Models.AI;
using Models.AI.Actions;
using Models.Map.Tiles;

namespace Models.Entities
{
    public abstract class LivingEntity : Entity
    {
        public float MovementSpeed { get; set; }
        
        protected float Health { get; set; }

        protected ActionManager actionManager;

        protected AIMotor motor;

        protected LivingEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 1.0f;
            Health = 100.0f;
            actionManager = new ActionManager();
            motor = new AIMotor(this);
        }

        public override void Update()
        {
            actionManager.Update();
            motor.Update();
        }
    }
}
