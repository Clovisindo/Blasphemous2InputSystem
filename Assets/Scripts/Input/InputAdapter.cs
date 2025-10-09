using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public enum InputDeviceType
    {
        KeyboardMouse,
        Gamepad
    }

    public class InputAdapter : IInputService
    {
        readonly Queue<InputCommand> _queue = new();
        IInputStrategy _strategy;
        PlayerInputActions _actionsAsset;

        private readonly IInputStrategy _keyboardStrategy;
        private readonly IInputStrategy _gamepadStrategy;
        private string _currentDeviceLayout = "Keyboard";
        private float _lastSwitchTime;
        private const float DEVICE_SWITCH_COOLDOWN = 0.25f; // segundos
        public event Action<string> OnDeviceChanged; // "Keyboard", "Gamepad"


        public InputAdapter(IInputStrategy keyboardStrategy, IInputStrategy gamepadStrategy)
        {
            _keyboardStrategy = keyboardStrategy;
            _gamepadStrategy = gamepadStrategy;
        }


        public void Initialize(PlayerInputActions actionsAsset) 
        {
            _actionsAsset = actionsAsset ?? throw new ArgumentNullException(nameof(actionsAsset));

            SetStrategy(_keyboardStrategy);//por defecto
        }


        public void Update(float deltaTime)
        {
            DetectDeviceChange();

            if (_strategy == null) return;
            var cmds = _strategy.Poll(deltaTime);
            if (cmds != null)
            {
                foreach (var c in cmds)
                    _queue.Enqueue(c);
            }
        }

        private void DetectDeviceChange()
        {
            var lastDevice = InputSystem.GetDevice<Keyboard>()?.wasUpdatedThisFrame == true
                ? "Keyboard"
                : InputSystem.GetDevice<Gamepad>()?.wasUpdatedThisFrame == true
                    ? "Gamepad"
                    : _currentDeviceLayout;

            if (lastDevice != _currentDeviceLayout &&
                Time.unscaledTime - _lastSwitchTime > DEVICE_SWITCH_COOLDOWN)
            {
                _lastSwitchTime = Time.unscaledTime;
                _currentDeviceLayout = lastDevice;
                OnDeviceChanged?.Invoke(lastDevice);
                SetStrategy(lastDevice == "Gamepad" ? _gamepadStrategy : _keyboardStrategy);
            }
        }

        //private void OnAnyInputPerformed(InputAction.CallbackContext ctx)
        //{
        //    var layout = ctx.control.device.layout;

        //    var newDevice = layout.Contains("XInputControllerWindows") ? "XInputControllerWindows" : "Keyboard";

        //    if (Time.unscaledTime - _lastSwitchTime < DEVICE_SWITCH_COOLDOWN)
        //        return;

        //    if (newDevice == "XInputControllerWindows" && ctx.control is StickControl stick)
        //    {
        //        if (stick.ReadValue().sqrMagnitude < 0.01f)
        //            return; 
        //    }

        //    if (newDevice != _currentDeviceLayout)
        //    {
        //        _currentDeviceLayout = newDevice;
        //        _lastSwitchTime = Time.unscaledTime;
        //        SetStrategy(newDevice == "XInputControllerWindows" ? _gamepadStrategy : _keyboardStrategy);
        //        OnDeviceChanged?.Invoke(_currentDeviceLayout);
        //    }

        //}

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
        }
    }
}
