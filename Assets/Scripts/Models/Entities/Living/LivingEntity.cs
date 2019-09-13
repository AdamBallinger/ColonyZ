using Models.AI;
using Models.AI.Actions;
using Models.Map.Tiles;

namespace Models.Entities.Living
{
    public abstract class LivingEntity : Entity
    {
        public float MovementSpeed { get; set; }
        
        public AIMotor Motor { get; }
        
        protected float Health { get; set; }

        protected ActionManager actionManager;

        protected LivingEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 1.0f;
            Health = 100.0f;
            actionManager = new ActionManager();
            Motor = new AIMotor(this);
            //CurrentTile.LivingEntities.Add(this);
        }

        public override void Update()
        {
            actionManager.Update();
            Motor.Update();
        }
    }
}
