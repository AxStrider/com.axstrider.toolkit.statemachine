namespace AxStrider.Toolkit.StateMachine
{
    public abstract class AxState : IState
    {
        public virtual void OnEnter()       { }
        public virtual void OnExit()        { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnUpdate()      { }
    }
}
