using UnityEngine;

namespace Axstrider.Toolkit.StateMachine
{
    [AddComponentMenu("StateMachine Toolkit/Debugger")]
    public class StateMachineDebugger : MonoBehaviour
    {
        [Tooltip("Glissez un composant ici ou laissez vide pour détecter automatiquement un composant sur ce GameObject.")]
        [SerializeField] private MonoBehaviour targetObject;

        private IStateMachineHolder holder;

        private void OnValidate()
        {
            // Vérifie dans l'inspecteur si le composant glissé est valide
            if (targetObject != null && !(targetObject is IStateMachineHolder))
            {
                Debug.LogWarning($"[StateMachineDebugger] {targetObject.GetType().Name} n'implémente pas IStateMachineHolder !", this);
            }
        }

        public string GetStatePath()
        {
            // 1. Si une cible spécifique est assignée
            if (targetObject is IStateMachineHolder targetHolder)
            {
                return targetHolder.GetStateMachine()?.GetActiveStatePath() ?? "StateMachine non initialisée";
            }

            // 2. Sinon, cherche automatiquement sur le même GameObject
            if (holder == null)
            {
                holder = GetComponent<IStateMachineHolder>();
            }

            if (holder != null && holder.GetStateMachine() != null)
            {
                return holder.GetStateMachine().GetActiveStatePath();
            }

            return "Aucun IStateMachineHolder détecté";
        }
    }
}
