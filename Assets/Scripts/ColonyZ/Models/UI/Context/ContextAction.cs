using System;

namespace ColonyZ.Models.UI.Context
{
    public class ContextAction
    {
        public string Name { get; }

        public Action Action { get; }

        public ContextAction(string _name, Action _action)
        {
            Name = _name;
            Action = _action;
        }
    }
}