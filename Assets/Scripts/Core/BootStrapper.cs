using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Settings;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        public PlayerSettingsSO playerSettings; // arrastra ScriptableObject en inspector
        public PlayerInputActions inputActionsAsset;
        KeyboardInputStrategy keyboardInputStrategy;
        GamepadInputStrategy gamepadInputStrategy;

        public static IContainer Container { get; private set; }
        public static event Action<PlayerInputActions> OnCoreInitialized;

        void Awake()
        {
            Container = new SimpleContainer();
            inputActionsAsset = new PlayerInputActions();
            inputActionsAsset.Enable();

            keyboardInputStrategy = new KeyboardInputStrategy();
            gamepadInputStrategy = new GamepadInputStrategy();
            // EventBus
            var eventBus = new EventBus();
            Container.RegisterSingleton<IEventBus>(eventBus);

            // Register settings
            Container.RegisterSingleton<PlayerSettingsSO>(playerSettings);

            // Input adapter y Strategy
            var adapter = new InputAdapter(keyboardInputStrategy, gamepadInputStrategy);
            adapter.Initialize(inputActionsAsset);
            adapter.OnDeviceChanged += OnDeviceChanged;
            Container.RegisterSingleton<InputAdapter>(adapter);
            Container.RegisterSingleton<IInputService>(adapter);

            // Crear estrategias y setear una por defecto
            var keyboardStrategy = new KeyboardInputStrategy();
            var gamepadStrategy = new GamepadInputStrategy();
            //adapter.SetStrategy(keyboardStrategy);
            //adapter.SetStrategy(gamepadStrategy); 
            OnCoreInitialized?.Invoke(inputActionsAsset);

            // Otros registros: PlayerStateMachine, InputBuffer...
            Container.RegisterSingleton<InputBuffer>(new InputBuffer(maxSize: 12, windowTime: 0.6f));
            Container.RegisterTransient(() => new PlayerStateMachine(Container.Resolve<PlayerSettingsSO>(), Container.Resolve<IEventBus>()));
        }

        private void OnDeviceChanged(string device )
        {
            //Debug.Log($"Dispositivo activo : {device}");
            //añadir avisos a UI para cambiar esquema botones
        }
    }
}
