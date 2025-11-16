using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Orchestrator
{
    public class CoreOrchestrator : MonoBehaviour
    {
        private static readonly List<ICoreDependent> _pendingDependents = new();
        private static bool _coreReady = false;

        public static void Register(ICoreDependent dependent)
        {
            if (_coreReady)
                dependent.OnCoreReady();
            else
                _pendingDependents.Add(dependent);
        }

        public static void NotifyCoreReady()
        {
            _coreReady = true;

            foreach (var dependent in _pendingDependents)
                dependent.OnCoreReady();

            _pendingDependents.Clear();
        }
    }
}
