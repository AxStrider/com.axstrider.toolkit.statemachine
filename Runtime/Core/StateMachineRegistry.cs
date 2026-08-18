using System.Collections.Generic;
using UnityEngine;

namespace AxStrider.Toolkit.StateMachine
{
    public static class StateMachineRegistry
    {
        private static readonly List<AxStateMachine> activeMachines = new();

        public static void Register(AxStateMachine machine)
        {
            if (machine != null && !activeMachines.Contains(machine))
                activeMachines.Add(machine);
        }

        /// <summary>
        /// Returns active state machines and automatically deletes those that are destroyed.
        /// </summary>
        public static IReadOnlyList<AxStateMachine> GetActiveMachines()
        {
            // Nettoyage automatique en 1 ligne : supprime les machines dont le GameObject est détruit
            activeMachines.RemoveAll(m => m == null || IsOwnerDestroyed(m.Owner));
            return activeMachines;
        }

        private static bool IsOwnerDestroyed(object owner)
        {
            // Surcharge de l'opérateur null d'Unity pour détecter un GameObject détruit
            if (owner is Object unityObject)
            {
                return unityObject == null;
            }
            return false;
        }
    }
}
