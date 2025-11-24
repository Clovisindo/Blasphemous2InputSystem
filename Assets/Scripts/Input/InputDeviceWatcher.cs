using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Game.Input
{
    public class InputDeviceWatcher : IDisposable, IInputDeviceWatcher
    {
        /// <summary>
        /// Aviso por evento a InputAdapter del cambio de dispositivo
        /// </summary>
        public event Action<InputDeviceType> OnDeviceChanged;
        private InputDeviceType _currentType = InputDeviceType.Unknown;

        public InputDeviceWatcher()
        {
            InputSystem.onEvent += OnInputEvent;
        }
        private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (device == null || !device.enabled) return;

            InputDeviceType newType = DetectDeviceType(device);
            if (newType == InputDeviceType.Unknown) return;

            if (newType != _currentType)
            {
                _currentType = newType;
                OnDeviceChanged?.Invoke(newType);
                Debug.Log($"[InputDeviceWatcher] Dispositivo activo : {_currentType}");
            }
        }

        private static InputDeviceType DetectDeviceType(InputDevice device)
        {
            if (device is Gamepad)
                return InputDeviceType.Gamepad;
            if (device is Keyboard or Mouse)
                return InputDeviceType.KeyboardMouse;
            return InputDeviceType.Unknown;
        }

        public void Dispose()
        {
            InputSystem.onEvent -= OnInputEvent;
        }
    }
}
