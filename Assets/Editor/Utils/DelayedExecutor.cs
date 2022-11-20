using System;
using System.Collections.Generic;

namespace NineEightOhThree.Editor.Utils
{
    public class DelayedExecutor
    {
        private Queue<Action> actions;

        public DelayedExecutor()
        {
            actions = new Queue<Action>();
        }

        public void Schedule(Action action)
        {
            actions.Enqueue(action);
        }

        public void ExecuteAll()
        {
            while (actions.TryDequeue(out Action action))
                action();
        }
    }
}