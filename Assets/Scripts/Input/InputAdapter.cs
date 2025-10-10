using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    public enum InputDeviceType
    {
        KeyboardMouse,
        Gamepad,
        Unknown
    }

    public class InputAdapter : IInputService
    {
        readonly Queue<InputCommand> _queue = new();
        IInputStrategy _strategy;
        PlayerInputActions _actionsAsset;
        private readonly IInputStrategy _keyboardStrategy;
        private readonly IInputStrategy _gamepadStrategy;
        /// <summary>
        /// solo bandera en evento cambiar dispositivo
        /// </summary>
        private IInputStrategy _currentStrategy;
        private readonly InputDeviceWatcher _watcher;

        public InputDeviceType CurrentDeviceType { get; private set; }

        public InputAdapter(IInputStrategy keyboardStrategy, IInputStrategy gamepadStrategy)
        {
            _keyboardStrategy = keyboardStrategy;
            _gamepadStrategy = gamepadStrategy;

            _currentStrategy = _keyboardStrategy;
            CurrentDeviceType = InputDeviceType.KeyboardMouse;

            _watcher = new InputDeviceWatcher();
            _watcher.OnDeviceChanged += OnDeviceChanged;
        }

     
        public void Initialize(PlayerInputActions actionsAsset) 
        {
            _actionsAsset = actionsAsset ?? throw new ArgumentNullException(nameof(actionsAsset));

            SetStrategy(_keyboardStrategy);//por defecto
        }

        public void Update(float deltaTime)
        {
            if (_strategy == null) return;
            var cmds = _strategy.Poll(deltaTime);
            if (cmds != null)
            {
                foreach (var c in cmds)
                    _queue.Enqueue(c);
            }
        }

        public void SetStrategy(IInputStrategy strategy)
        {
            _strategy?.ShutDown();
            _strategy = strategy;
            if (_strategy != null) 
                _strategy.Initialize(_actionsAsset);

            Debug.Log($"[InputAdapter] Cambiado a estrategia: {_strategy.GetType().Name}");

        }

        public bool TryDequeue(out InputCommand command)
        {
            if(_queue.Count > 0) { command = _queue.Dequeue(); return true; }
            command = null;
            return false;
        }

        public void Enqueue(InputCommand command) => _queue.Enqueue(command);

        public void ShutDown()
        {
            _strategy?.ShutDown();
            _queue.Clear();
            _watcher.Dispose();
        }

        private void OnDeviceChanged(InputDeviceType newType)
        {
            IInputStrategy newStrategy = newType switch
            {
                InputDeviceType.Gamepad => _gamepadStrategy,
                InputDeviceType.KeyboardMouse => _keyboardStrategy,
                _ => _keyboardStrategy
            };

            if (_currentStrategy == newStrategy) return;

            _currentStrategy = newStrategy;
            SetStrategy(newStrategy);
            CurrentDeviceType = newType;
        }

    }
}
