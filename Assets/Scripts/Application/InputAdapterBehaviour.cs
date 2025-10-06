using Game.Core;
using UnityEngine;

namespace Game.Input
{
    public class InputAdapterBehaviour : MonoBehaviour
    {
        public InputAdapter Adapter { get; private set; }

        private void Awake()
        {
            Adapter = Bootstrapper.Container.Resolve<InputAdapter>();
            Adapter.Initialize();
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
