namespace ColonyZ.Models.UI.Context
{
    public interface IContextProvider
    {
        ContextAction[] GetContextActions();

        string GetContextMenuName();
    }
}