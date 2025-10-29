using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;
using static Utilities;

namespace Game.Input
{
    public class GamepadInputStrategy : IInputStrategy
    {
        private readonly Func<InputDevice, bool> _deviceFilter = device => device is Gamepad;
        PlayerInputActions _actions;
        readonly Queue<InputCommand> _attackQueue = new();
        readonly Queue<InputCommand> _jumpQueue = new();
        public InputDeviceType DeviceType => InputDeviceType.Gamepad;

        public void Initialize(PlayerInputActions actions)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _actions.Gameplay.Movement.performed += OnMovementPerformed;
            _actions.Gameplay.Movement.canceled += OnMovementCanceled;
            _actions.Gameplay.Attack.performed += OnAttackPerformed;
            _actions.Gameplay.Jump.performed += OnJumpPerformed;
            _actions.Gameplay.Dash.performed += OnDashPerformed;
            _actions.Enable();
        }

       

        public List<InputCommand> Poll(float deltaTime)
        {
            var outlist = new List<InputCommand>();

            var v = _actions.Gameplay.Movement.ReadValue<Vector2>();

            if(v.sqrMagnitude > 0.001f)
                outlist.Add(new MovementCommand(v,Time.unscaledDeltaTime));
            else
                outlist.Add(new MovementCommand(Vector2.zero, Time.unscaledDeltaTime));

            if (_attackQueue.Count > 0)
            {
                while (_attackQueue.Count > 0) outlist.Add(_attackQueue.Dequeue());
            }

            if (_jumpQueue.Count > 0)
            {
                while (_jumpQueue.Count > 0) outlist.Add(_jumpQueue.Dequeue());
            }

            return outlist;
        }

        void OnMovementPerformed(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
        }

        void OnMovementCanceled(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
        }

        void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            if (!_deviceFilter(ctx.control.device)) return;
            var type = ctx.interaction is HoldInteraction ? AttackType.Heavy : AttackType.Light;
            _attackQueue.Enqueue(new AttackCommand(type, Time.unscaledTime));
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
