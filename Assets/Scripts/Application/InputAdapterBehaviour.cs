using Game.Core;
using UnityEngine;

namespace Game.Input
{
    public class InputAdapterBehaviour : MonoBehaviour
    {
        public InputAdapter Adapter { get; private set; }

        private void Awake()
        {
            Bootstrapper.OnCoreInitialized += OnCoreReady;
        }

        private void OnCoreReady(PlayerInputActions actions)
        {
            Adapter = Bootstrapper.Container.Resolve<InputAdapter>();
            //Adapter.Initialize(actions);
            Bootstrapper.OnCoreInitialized -= OnCoreReady;
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
