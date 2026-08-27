using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace AxStrider.Toolkit.StateMachine
{
    public class AxStateMachine : IDisposable
    {
        public object Owner { get; }
        public string Name  { get; }
        public IState CurrentState { get; private set; }

        private readonly Dictionary<Type, List<Transition>> transitions = new();
        private readonly List<Transition> anyTransitions = new();

        public event Action<IState, IState> OnStateChanged;


        // ──────────────────────────────────────────────────────────
        // 1. Constructor
        // ──────────────────────────────────────────────────────────
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

            var previous = CurrentState;

            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();

            OnStateChanged?.Invoke(previous, CurrentState);
        }

        public void ClearCurrentState()
        {
            CurrentState = null;
        }

        public void Dispose()
        {
            StateMachineRegistry.Unregister(this);
        }

        #endregion


        // ==========================================================
        //                       Transitions
        // ==========================================================
        #region Transitions

        public void AddTransition(IState from, IState to, Func<bool> condition, string label = null)
        {
            if (from == null)      throw new ArgumentNullException(nameof(from));
            if (to == null)        throw new ArgumentNullException(nameof(to));
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var type = from.GetType();
            if (!transitions.ContainsKey(type))
            {
                transitions[type] = new List<Transition>();
            }
            transitions[type].Add(new Transition(to, condition, label));
        }

        public void AddAnyTransition(IState to, Func<bool> condition, string label = null)
        {
            if (to == null)        throw new ArgumentNullException(nameof(to));
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            anyTransitions.Add(new Transition(to, condition, label));
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

        /// <summary>Read-only view of the transitions registered from a given state type.
        /// For tooling (debug windows, graph export) — not meant to drive gameplay logic.</summary>
        public IReadOnlyList<Transition> GetTransitionsFrom(Type stateType)
        {
            return transitions.TryGetValue(stateType, out var list) ? list : Array.Empty<Transition>();
        }

        /// <summary>Read-only view of the transitions that can fire from any state.</summary>
        public IReadOnlyList<Transition> AnyTransitions => anyTransitions;

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

        /// <summary>
        /// Dumps the registered transitions as Mermaid state-diagram syntax — paste the
        /// result into https://mermaid.live (or a markdown file that renders Mermaid) for
        /// a visual map of this machine's flow. A HierarchicalState target shows up as a
        /// single node here; call ToMermaid() on its own SubStateMachine for its nested
        /// flow, since a static transition list has no live instance to recurse into.
        /// </summary>
        public string ToMermaid()
        {
            var sb = new StringBuilder();
            sb.AppendLine("stateDiagram-v2");

            foreach (var kvp in transitions)
            {
                var fromName = kvp.Key.Name;
                foreach (var t in kvp.Value)
                {
                    var edge = $"    {fromName} --> {t.TargetState.GetType().Name}";
                    sb.AppendLine(string.IsNullOrEmpty(t.Label) ? edge : $"{edge} : {t.Label}");
                }
            }

            foreach (var t in anyTransitions)
            {
                var note = $"    note right of {t.TargetState.GetType().Name} : from ANY state";
                sb.AppendLine(string.IsNullOrEmpty(t.Label) ? note : $"{note} ({t.Label})");
            }

            return sb.ToString();
        }

        #endregion
    }
}
