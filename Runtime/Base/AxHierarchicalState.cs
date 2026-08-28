namespace AxStrider.Toolkit.StateMachine
{
    public class AxHierarchicalState: AxState
    {
        public AxStateMachine SubStateMachine { get; } = new AxStateMachine();

        public bool ResetSubStateOnEnter { get; set; } = true;

        private IState initialSubState;


        public void SetInitialSubState(IState state)
        {
            initialSubState = state;
        }


        // ──────────────────────────────────────────────────────────
        // Lifecycle
        // ──────────────────────────────────────────────────────────
        #region Lifecycle

        public override void OnEnter()
        {
            base.OnEnter();

            if (ResetSubStateOnEnter || SubStateMachine.CurrentState == null)
            {
                if (initialSubState != null)
                {
                    SubStateMachine.SetState(initialSubState);
                }
            }
            else
            {
                SubStateMachine.CurrentState.OnEnter();
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

            if (ResetSubStateOnEnter)
            {
                SubStateMachine.ClearCurrentState();
            }

            base.OnExit();
        }

        #endregion
    }
}
