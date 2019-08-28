using Models.Jobs;
using Models.Map;
using Models.Map.Tiles;

namespace Models.Entities.Living
{
    public class HumanEntity : LivingEntity
    {
        public Job CurrentJob { get; private set; }
        
        public HumanEntity(Tile _tile) : base(_tile)
        {
            
        }
        
        public bool SetJob(Job _job)
        {
            if (CurrentJob != null && CurrentJob.Progress < 1.0f) return false;

            CurrentJob = _job;
            Motor.Stop();
            return true;
        }

        public override void Update()
        {
            base.Update();
            
            if (!Motor.Working && CurrentJob == null)
            {
                Motor.SetTargetTile(World.Instance.GetRandomTile());
            }
            
            CurrentJob?.Update();
        }
    }
}
