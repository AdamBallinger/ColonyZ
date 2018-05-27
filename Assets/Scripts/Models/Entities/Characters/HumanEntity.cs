using Models.AI;
using Models.Map;

namespace Models.Entities.Characters
{
    public class HumanEntity : CharacterEntity
    {

        public HumanEntity(Tile _tile) : base(_tile)
        {
            actionManager.RegisterActionFinishCallback(OnActionFinish);
            actionManager.Queue(new MoveAction(this, World.Instance.GetRandomTile()));
        }

        public void OnActionFinish(BaseAction _action)
        {
            if(_action is MoveAction)
            {
                actionManager.Queue(new MoveAction(this, World.Instance.GetRandomTile()));
            }
        }
    }
}
