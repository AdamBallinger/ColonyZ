using System;
using System.Collections.Generic;
using Models.Entities;

namespace Models.AI
{
    public class ActionManager
    {
        public BaseAction DefaultAction { get; set; }

        private List<BaseAction> ActionQueue { get; }

        private BaseAction CurrentAction { get; set; }

        private Action<BaseAction> actionStartCallback;
        private Action<BaseAction> actionFinishCallback;

        public ActionManager()
        {
            ActionQueue = new List<BaseAction>();
        }

        /// <summary>
        /// Adds a new action to the end of the action queue.
        /// </summary>
        /// <param name="_baseAction"></param>
        public void Queue(BaseAction _baseAction)
        {
            ActionQueue.Add(_baseAction);
        }

        public void ClearQueue()
        {
            ActionQueue.Clear();
        }

        /// <summary>
        /// Forces the current action to finish early and sets the current action to the provided action immediately.
        /// </summary>
        /// <param name="_action"></param>
        public void Interrupt(BaseAction _action)
        {
            if(CurrentAction != null)
            {
                ActionFinished();
            }

            CurrentAction = _action;
            ActionStart();
        }

        private void ActionStart()
        {
            if (CurrentAction == null)
            {
                return;
            }

            CurrentAction.OnStart();
            actionStartCallback?.Invoke(CurrentAction);
        }

        private void ActionFinished()
        {
            if(CurrentAction == null)
            {
                return;
            }

            CurrentAction.OnFinish();
            actionFinishCallback?.Invoke(CurrentAction);
            CurrentAction = null;
        }

        public void Update()
        {
            if(CurrentAction != null && CurrentAction.HasFinished())
            {
                ActionFinished();
            }

            if(CurrentAction == null)
            {
                if(ActionQueue.Count > 0)
                {
                    CurrentAction = ActionQueue[0];
                    ActionQueue.RemoveAt(0);
                    ActionStart();
                }
                else
                {
                    CurrentAction = DefaultAction;
                    ActionStart();
                }

                return;
            }

            CurrentAction?.OnUpdate();
        }

        public void RegisterActionStartCallback(Action<BaseAction> _callback)
        {
            actionStartCallback += _callback;
        }

        public void RegisterActionFinishCallback(Action<BaseAction> _callback)
        {
            actionFinishCallback += _callback;
        }
    }
}
