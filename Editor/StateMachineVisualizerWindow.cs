#if UNITY_EDITOR
using Axstrider.Toolkit.StateMachine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AxStrider.Toolkit.StateMachine.EditorOnly
{
    public class StateMachineVisualizerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string searchFilter = "";

        private readonly List<AxStateMachine> cachedMachines = new List<AxStateMachine>();

        [MenuItem("Window/AxStrider/StateMachine Toolkit/Live Visualizer")]
        [MenuItem("Tools/AxStrider/StateMachine Toolkit/Live Visualizer")]
        public static void OpenWindow()
        {
            var window = GetWindow<StateMachineVisualizerWindow>("State Machines Visualizer");
            window.minSize = new Vector2(450, 350);
            window.Show();
        }

        private void OnEnable()
        {
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
            if (Application.isPlaying)
            {
                cachedMachines.Clear();
                cachedMachines.AddRange(StateMachineRegistry.GetActiveMachines());
            }

            DrawHeader();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Passez en mode Play pour voir les State Machines actives en temps réel.", MessageType.Info);
                return;
            }

            DrawSearchBar();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUIStyle cardStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(0, 0, 4, 4)
            };

            for (int i = 0; i < cachedMachines.Count; i++)
            {
                var machine = cachedMachines[i];
                if (machine == null) continue;

                string objName = machine.Name;

                if (!string.IsNullOrEmpty(searchFilter) &&
                    !objName.ToLower().Contains(searchFilter.ToLower()))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical(cardStyle);

                if (machine.Owner is MonoBehaviour mono && mono != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(objName, EditorStyles.boldLabel);

                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = mono.gameObject;
                        EditorGUIUtility.PingObject(mono.gameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField(objName, EditorStyles.boldLabel);
                }

                GUI.backgroundColor = new Color(0.2f, 0.8f, 0.4f, 0.3f);
                EditorGUILayout.LabelField($"État actuel : {machine.GetActiveStatePath()}", EditorStyles.textField);
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("State Machines Actives", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total en cours : {(Application.isPlaying ? cachedMachines.Count : 0)}", EditorStyles.miniLabel);
            EditorGUILayout.Space(5);
        }

        private void DrawSearchBar()
        {
            searchFilter = EditorGUILayout.TextField("Rechercher :", searchFilter);
            EditorGUILayout.Space(5);
        }
    }
}
#endif
