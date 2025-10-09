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
        public InputDeviceType DeviceType => InputDeviceType.KeyboardMouse;
        PlayerInputActions _actions;
        Vector2 _currentMovement = Vector2.zero;
        bool _movementDirty = false;
        readonly Queue<InputCommand> _attackQueue = new();

        public void Initialize(PlayerInputActions actionsAsset)
        {
            _actions = actionsAsset ?? throw new ArgumentNullException(nameof(actionsAsset));

            _actions.Gameplay.Movement.performed += OnMovementPerformed;
            _actions.Gameplay.Movement.canceled += OnMovementCanceled;
            _actions.Gameplay.Movement.performed += ctx =>
            {
                var value = ctx.ReadValue<Vector2>();
                Debug.Log($"[Movement] device={ctx.control.device.displayName}, value={value}");
            };

            _actions.Gameplay.Attack.performed += OnAttackPerformed;

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

            return outList;
        }

        public void ShutDown()
        {
            if (_actions != null)
            {
                _actions.Gameplay.Movement.performed -= OnMovementPerformed;
                _actions.Gameplay.Movement.canceled -= OnMovementCanceled;
                _actions.Gameplay.Attack.performed -= OnAttackPerformed;
                _actions.Disable();
            }
        }

        void OnMovementPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device is not Keyboard)
                return;
            _currentMovement = ctx.ReadValue<Vector2>();
            _movementDirty = true;
        }

        void OnMovementCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device is not Keyboard)
                return;
            _currentMovement = Vector2.zero;
            _movementDirty = true;
        }

        void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device is not Keyboard)
                return;
            var type = ctx.interaction is HoldInteraction ? AttackType.Heavy : AttackType.Light;
            _attackQueue.Enqueue(new AttackCommand(type, Time.unscaledTime));
        }
    }
}
