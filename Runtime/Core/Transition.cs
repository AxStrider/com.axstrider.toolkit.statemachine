using System;

namespace AxStrider.Toolkit.StateMachine
{
    public class Transition
    {
        public IState     TargetState { get; }
        public Func<bool> Condition   { get; }
        public string     Label       { get; }

        public Transition(IState targetState, Func<bool> condition, string label = null)
        {
            TargetState = targetState;
            Condition   = condition;
            Label       = label;
        }
    }
}
