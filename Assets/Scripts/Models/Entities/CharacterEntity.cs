using Models.AI.Actions;
using Models.Map.Tiles;

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
