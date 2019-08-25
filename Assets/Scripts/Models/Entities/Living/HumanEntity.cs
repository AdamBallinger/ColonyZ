using Models.Jobs;
using Models.Map;
using Models.Map.Tiles;

namespace Models.Entities.Living
{
    public class HumanEntity : LivingEntity
    {
        public Job Job { get; private set; }
        
        public HumanEntity(Tile _tile) : base(_tile)
        {
            motor.SetTargetTile(World.Instance.GetRandomTile());
        }
        
        public bool SetJob(Job _job)
        {
            if (Job != null) return false;

            Job = _job;
            return true;
        }

        public override void Update()
        {
            base.Update();
            
            if (!motor.Working && Job == null)
            {
                motor.SetTargetTile(World.Instance.GetRandomTile());
            }

            Job?.Update();
        }
    }
}
