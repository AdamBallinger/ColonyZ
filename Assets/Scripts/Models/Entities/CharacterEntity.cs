
namespace Models.Entities
{
    public class CharacterEntity : Entity
    {

        public float MovementSpeed { get; protected set; }

	    public CharacterEntity(int _x, int _y) : base(_x, _y)
	    {
	        MovementSpeed = 1.0f;
	    }

        public override void Update()
        {
            
        }
    }
}
