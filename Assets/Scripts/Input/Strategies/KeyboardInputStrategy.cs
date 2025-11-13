using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static Utilities;

namespace Game.Input
{
    public class KeyboardInputStrategy : IInputStrategy
    {
        private readonly Func<InputDevice, bool> _deviceFilter = device => device is Keyboard;
        readonly Queue<InputCommand> _attackQueue = new();
        readonly Queue<InputCommand> _jumpQueue = new();
        PlayerInputActions _actions;
        Vector2 _currentMovement = Vector2.zero;
        /// <summary>
        /// flag para marcar que el movimiento ha cambiado desde el ultimo poll() cuando se para el movimiento al soltar teclas
        /// </summary>
        bool _movementDirty = false;
        public InputDeviceType DeviceType => InputDeviceType.KeyboardMouse;

        public void Initialize(PlayerInputActions actionsAsset)
        {
            _actions = actionsAsset ?? throw new ArgumentNullException(nameof(actionsAsset));

            _actions.Gameplay.Movement.performed += OnMovementPerformed;
            _actions.Gameplay.Movement.canceled += OnMovementCanceled;
            _actions.Gameplay.Attack.performed += OnAttackPerformed;
            _actions.Gameplay.Jump.performed += OnJumpPerformed;
            _actions.Gameplay.Dash.performed += OnDashPerformed;

            _actions.Enable();
        }

       

        public List<InputCommand> Poll(float deltaTime)
        {
             var outList = new List<InputCommand>();

            if(_currentMovement.sqrMagnitude > 0.001f)
            {
                outList.Add(new MovementCommand(_currentMovement,Time.unscaledDeltaTime));
                _movementDirty = false;
            }
            else if (_movementDirty)
            {
                outList.Add(new MovementCommand(Vector2.zero, Time.unscaledDeltaTime));
                _movementDirty = false;
            }
            while (_attackQueue.Count > 0)
            {
                outList.Add(_attackQueue.Dequeue());
            }
            while (_jumpQueue.Count > 0)
            {
                outList.Add(_jumpQueue.Dequeue());
            }

            return outList;
        }

        void OnMovementPerformed(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
            _currentMovement = ctx.ReadValue<Vector2>();
            _movementDirty = true;
        }

        void OnMovementCanceled(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
            _currentMovement = Vector2.zero;
            _movementDirty = true;
        }

        void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
            var type = ctx.interaction is HoldInteraction ? AttackType.Heavy : AttackType.Light;
            _attackQueue.Enqueue(new AttackCommand(type, Time.unscaledDeltaTime));
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
            _jumpQueue.Enqueue(new JumpCommand(Time.unscaledDeltaTime));
        }

        private void OnDashPerformed(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
            _jumpQueue.Enqueue(new DashCommand(Time.unscaledDeltaTime));
        }
        public void ShutDown()
        {
            if (_actions != null)
            {
                _actions.Gameplay.Movement.performed -= OnMovementPerformed;
                _actions.Gameplay.Movement.canceled -= OnMovementCanceled;
                _actions.Gameplay.Attack.performed -= OnAttackPerformed;
                _actions.Gameplay.Jump.performed -= OnJumpPerformed;
                _actions.Gameplay.Dash.performed -= OnDashPerformed;
                _actions.Disable();
            }
        }
    }
}
