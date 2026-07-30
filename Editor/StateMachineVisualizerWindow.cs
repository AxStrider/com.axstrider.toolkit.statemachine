#if UNITY_EDITOR
using Axstrider.Toolkit.StateMachine;
using UnityEditor;
using UnityEngine;

namespace AxStrider.Toolkit.StateMachine.EditorOnly
{
    public class StateMachineVisualizerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string searchFilter = "";

        [MenuItem("Tools/StateMachine Toolkit/Live Visualizer")]
        public static void OpenWindow()
        {
            var window = GetWindow<StateMachineVisualizerWindow>("State Machines Visualizer");
            window.minSize = new Vector2(450, 350);
            window.Show();
        }

        private void OnEnable()
        {
            // Demande le rafraîchissement continu de la fenêtre pendant le jeu
            EditorApplication.update += RepaintWindowDuringPlay;
        }

        private void OnDisable()
        {
            EditorApplication.update -= RepaintWindowDuringPlay;
        }

        private void RepaintWindowDuringPlay()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawHeader();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Passez en mode Play pour voir les State Machines actives en temps réel.", MessageType.Info);
                return;
            }

            DrawSearchBar();
            DrawStateMachineList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("State Machines Actives", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total en cours d'exécution : {StateMachineRegistry.GetActiveMachines().Count}", EditorStyles.miniLabel);
            EditorGUILayout.Space(5);
        }                                                         

        private void DrawSearchBar()
        {
            searchFilter = EditorGUILayout.TextField("Rechercher (Nom/Objet) :", searchFilter);
            EditorGUILayout.Space(5);
        }

        private void DrawStateMachineList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUIStyle cardStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(0, 0, 4, 4)
            };

            foreach (var machine in StateMachineRegistry.GetActiveMachines())
            {
                EditorGUILayout.BeginVertical(cardStyle);

                // Nom et bouton Ping si le owner est un MonoBehaviour
                if (machine.Owner is MonoBehaviour mono)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(machine.Name, EditorStyles.boldLabel);

                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = mono.gameObject;
                        EditorGUIUtility.PingObject(mono.gameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField(machine.Name, EditorStyles.boldLabel);
                }

                // Chemin d'état
                EditorGUILayout.LabelField($"État actuel : {machine.GetActiveStatePath()}", EditorStyles.textField);
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
