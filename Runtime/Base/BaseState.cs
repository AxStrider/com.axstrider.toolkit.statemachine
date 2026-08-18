namespace AxStrider.Toolkit.StateMachine
{
    public abstract class BaseState : IState
    {
        public virtual void OnEnter()       { }
        public virtual void OnExit()        { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnUpdate()      { }
    }
}
