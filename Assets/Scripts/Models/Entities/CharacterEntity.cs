using Models.AI;
using Models.Map;

namespace Models.Entities
{
    public abstract class CharacterEntity : Entity
    {
        public float MovementSpeed { get; set; }

        protected ActionManager actionManager;

        protected CharacterEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 1.0f;
            actionManager = new ActionManager();
        }

        public override void Update()
        {
            actionManager.Update();
        }
    }
}
