using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Settings;

using UnityEngine;

namespace Game.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        public PlayerSettingsSO playerSettings; // arrastra ScriptableObject en inspector
        public PlayerInputActions inputActionsAsset; // arrastra el asset .inputactions (opcional)

        public static IContainer Container { get; private set; }

        void Awake()
        {
            Container = new SimpleContainer();

            // EventBus
            var eventBus = new EventBus();
            Container.RegisterSingleton<IEventBus>(eventBus);

            // Register settings
            Container.RegisterSingleton<PlayerSettingsSO>(playerSettings);

            //Input service(se registra instancia)
            var inputSvc = new UnityInputService(inputActionsAsset, eventBus);
            inputSvc.Initialize(); // habilita el Input System
            Container.RegisterSingleton<IInputService>(inputSvc);

            // Otros registros: PlayerStateMachine, InputBuffer...
            Container.RegisterSingleton<InputBuffer>(new InputBuffer(maxSize: 12, windowTime: 0.6f));
            Container.RegisterTransient(() => new PlayerStateMachine(Container.Resolve<PlayerSettingsSO>(), Container.Resolve<IEventBus>()));
        }
    }
}
