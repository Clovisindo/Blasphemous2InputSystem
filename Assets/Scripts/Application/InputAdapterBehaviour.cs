using Game.Core;
using Game.Core.Orchestrator;
using UnityEngine;

namespace Game.Input
{
    public class InputAdapterBehaviour : MonoBehaviour,ICoreDependent
    {
        public InputAdapter Adapter { get; private set; }

        private void Awake()
        {
            CoreOrchestrator.Register(this);
        }

        public void OnCoreReady()
        {
            Adapter = Bootstrapper.Container.Resolve<InputAdapter>();
        }

        private void Update()
        {
            Adapter.Update(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            Adapter.ShutDown();
        }

       
    }
}
