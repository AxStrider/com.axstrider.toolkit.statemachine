namespace Axstrider.Toolkit.StateMachine
{
    public class HierarchicalState: BaseState
    {
        public AxStateMachine SubStateMachine { get; } = new AxStateMachine();

        private IState initialSubState;

        public void SetInitialSubState(IState state)
        {
            initialSubState = state;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (initialSubState != null)
            {
                SubStateMachine.ChangeState(initialSubState);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            SubStateMachine.Update();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            SubStateMachine.FixedUpdate();
        }

        public override void OnExit()
        {
            SubStateMachine.CurrentState?.OnExit();
            base.OnExit();
        }
    }
}
