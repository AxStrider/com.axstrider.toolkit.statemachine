#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AxStrider.Toolkit.StateMachine.Editor
{
    /// <summary>
    /// Draws the active state path above every GameObject that owns an active
    /// AxStateMachine (Owner set to a MonoBehaviour), directly in the Scene view.
    /// Useful the moment you have several AI-driven objects at once — no need to
    /// select each one to see what it's doing.
    /// </summary>
    [InitializeOnLoad]
    public static class StateMachineSceneOverlay
    {
        static StateMachineSceneOverlay()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!Application.isPlaying) return;

            foreach (var machine in StateMachineRegistry.GetActiveMachines())
            {
                if (machine.Owner is not MonoBehaviour mono) continue;

                var position = mono.transform.position + Vector3.up * 2f;
                Handles.Label(position, machine.GetActiveStatePath(), EditorStyles.whiteBoldLabel);
            }
        }
    }
}
#endif
