#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AxStrider.Toolkit.StateMachine.Editor
{
    /// <summary>
    /// Lists every currently active AxStateMachine (via StateMachineRegistry) with its
    /// live state path (GetActiveStatePath) and the outgoing transitions available from
    /// the current state. Subscribes to OnStateChanged on each machine it finds, so the
    /// window only repaints when a state actually changes — no per-frame polling.
    /// </summary>
    public class StateMachineDebugWindow : EditorWindow
    {
        private readonly HashSet<AxStateMachine> _subscribed = new();

        [MenuItem("AxStrider/State Machine Debugger")]
        private static void Open() => GetWindow<StateMachineDebugWindow>("State Machines");

        private void OnEnable() => EditorApplication.update += TrackActiveMachines;

        private void OnDisable()
        {
            EditorApplication.update -= TrackActiveMachines;
            foreach (var machine in _subscribed)
                machine.OnStateChanged -= OnAnyMachineStateChanged;
            _subscribed.Clear();
        }

        // The registry only grows/shrinks as machines are created/destroyed in Play
        // Mode, so we just re-scan each editor tick and diff subscriptions, instead of
        // hooking into Register/Unregister directly (which aren't events).
        private void TrackActiveMachines()
        {
            var active = new HashSet<AxStateMachine>(StateMachineRegistry.GetActiveMachines());

            foreach (var machine in active)
            {
                if (_subscribed.Add(machine))
                    machine.OnStateChanged += OnAnyMachineStateChanged;
            }

            _subscribed.RemoveWhere(machine =>
            {
                if (active.Contains(machine)) return false;
                machine.OnStateChanged -= OnAnyMachineStateChanged;
                return true;
            });
        }

        private void OnAnyMachineStateChanged(IState previous, IState current) => Repaint();

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to see active state machines.", MessageType.Info);
                return;
            }

            var machines = StateMachineRegistry.GetActiveMachines();

            if (machines.Count == 0)
            {
                EditorGUILayout.HelpBox("No active state machines.", MessageType.Info);
                return;
            }

            foreach (var machine in machines)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(machine.Name, machine.GetActiveStatePath(), EditorStyles.boldLabel);
                if (GUILayout.Button("Copy Mermaid", GUILayout.Width(100)))
                    EditorGUIUtility.systemCopyBuffer = machine.ToMermaid();
                EditorGUILayout.EndHorizontal();

                if (machine.CurrentState == null) continue;

                // Conditions are assumed cheap and side-effect-free, since OnGUI may
                // evaluate them several times per repaint.
                foreach (var t in machine.GetTransitionsFrom(machine.CurrentState.GetType()))
                {
                    var name = string.IsNullOrEmpty(t.Label) ? t.TargetState.GetType().Name : t.Label;
                    EditorGUILayout.LabelField("    → " + name, t.Condition() ? "ready" : "");
                }

                EditorGUILayout.Space(4);
            }
        }
    }
}
#endif
