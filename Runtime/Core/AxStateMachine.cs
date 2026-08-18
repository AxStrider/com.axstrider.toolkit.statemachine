using System;
using System.Collections.Generic;
using UnityEngine;


namespace AxStrider.Toolkit.StateMachine
{
    public class AxStateMachine
    {
        public object Owner { get; }
        public string Name  { get; }
        public IState CurrentState { get; private set; }

        private readonly Dictionary<Type, List<Transition>> transitions = new();
        private readonly List<Transition> anyTransitions = new();


        // ==========================================================
        //                      Constructor
        // ==========================================================
        #region Constructor

        public AxStateMachine(object owner = null, string customName = null) 
        { 
            Owner = owner;

            if (!string.IsNullOrEmpty(customName))
            {
                Name = customName;
            }
#if UNITY_EDITOR
            else if (owner is MonoBehaviour mono)
            {
                Name = $"{mono.gameObject.name} ({mono.GetType().Name})";
            }
#endif
            else if (owner != null)
            {
                Name = owner.GetType().Name;
            }
            else
            {
                Name = "StateMachine Anonymous";
            }

            StateMachineRegistry.Register(this);
        }

        #endregion


        // ==========================================================
        //                       Lifecycle
        // ==========================================================
        #region Lifecycle

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.TargetState);
            }

            CurrentState?.OnUpdate();
        }

        public void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }

        public void ChangeState(IState newState)
        {
            if (newState == CurrentState)
                return;

            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }

        #endregion


        // ==========================================================
        //                       Transitions
        // ==========================================================
        #region Transitions

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            var type = from.GetType();
            if (!transitions.ContainsKey(type))
            {
                transitions[type] = new List<Transition>();
            }
            transitions[type].Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            anyTransitions.Add(new Transition(to, condition));
        }

        private Transition GetTransition()
        {
            foreach (var t in anyTransitions)
            {
                if (t.Condition()) return t;
            }

            if (CurrentState != null && transitions.TryGetValue(CurrentState.GetType(), out var currentTransitions))
            {
                foreach (var t in currentTransitions)
                {
                    if (t.Condition()) return t;
                }
            }

            return null;
        }

        #endregion


        // ==========================================================
        //                        Helpers
        // ==========================================================
        #region Helpers

        /// <summary>
        /// Returns the full hierarchy as text: "Parent > SousEtat > SubSousEtat"
        /// </summary>
        public string GetActiveStatePath()
        {
            if (CurrentState == null ) return "None (null)";

            string currentName = CurrentState.GetType().Name;

            if (CurrentState is HierarchicalState hState)
            {
                return $"{currentName}  ►  {hState.SubStateMachine.GetActiveStatePath()}";
            }

            return currentName;
        }

        #endregion
    }
}
