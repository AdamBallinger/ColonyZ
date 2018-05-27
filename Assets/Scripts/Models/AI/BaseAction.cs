namespace Models.AI
{
    public abstract class BaseAction 
    {
        public virtual void OnStart() { }

        public virtual void OnFinish() { }

        public abstract void OnUpdate();

        public abstract bool HasFinished();
    }
}
