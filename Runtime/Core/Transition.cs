using System;

namespace Axstrider.Toolkit.StateMachine
{
    public class Transition
    {
        public IState     TargetState { get; }
        public Func<bool> Condition   { get; }

        public Transition(IState targetState, Func<bool> condition)
        {
            TargetState = targetState;
            Condition   = condition;
        }
    }
}
