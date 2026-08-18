#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AxStrider.Toolkit.StateMachine.EditorOnly
{
    [CustomEditor(typeof(StateMachineDebugger))]
    public class StateMachineDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dessine les champs par défaut (référence au GameLoop, etc.)
            DrawDefaultInspector();

            StateMachineDebugger debugger = (StateMachineDebugger)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Débugger de State Machine", EditorStyles.boldLabel);

            // N'exécuter l'affichage dynamique que si le jeu tourne
            if (Application.isPlaying)
            {
                // Style personnalisé pour la boîte d'état
                GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    alignment = TextAnchor.MiddleCenter
                };

                // Couleur verte d'état actif
                GUI.backgroundColor = new Color(0.2f, 0.8f, 0.4f, 0.3f);

                string path = debugger.GetStatePath();

                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.LabelField("Hiérarchie Actuelle :", EditorStyles.miniBoldLabel);
                EditorGUILayout.SelectableLabel(path, EditorStyles.boldLabel, GUILayout.Height(20));
                EditorGUILayout.EndVertical();

                GUI.backgroundColor = Color.white;

                // Force la mise à jour de l'Inspecteur à chaque frame
                Repaint();
            }
            else
            {
                EditorGUILayout.HelpBox("Lancez le mode Play pour voir l'état actif en direct.", MessageType.Info);
            }
        }
    }
}
#endif