using Game.Events;
using Game.Input.Commands;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using static Utilities;

namespace Game.Input
{
    public class UnityInputService : IInputService
    {
        readonly Queue<InputCommand> _queue = new();
        PlayerInputActions _actions;
        readonly IEventBus _eventBus;

        Vector2 _currentMovement = Vector2.zero;

        public UnityInputService(PlayerInputActions actionsAsset, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _actions = actionsAsset ?? new PlayerInputActions();

            _actions.Gameplay.Movement.performed += ctx =>
            {
                var val = ctx.ReadValue<Vector2>();
                Debug.Log($"Movement input: {val}");
                _currentMovement = ctx.ReadValue<Vector2>();
            };

            _actions.Gameplay.Movement.canceled += ctx => _currentMovement = Vector2.zero;

            _actions.Gameplay.Attack.performed += ctx =>
            {
                var at = ctx.interaction is HoldInteraction ? AttackType.Heavy : AttackType.Light;
                Enqueue(new AttackCommand(at, Time.unscaledTime));
            };
        }

        public void Initialize()
        {
            _actions.Enable();
        }

        public void Enqueue(InputCommand command) => _queue.Enqueue(command);


        public bool TryDequeue(out InputCommand command)
        {
            if (_queue.Count > 0)
            {
                command = _queue.Dequeue();
                return true;
            }
            command = null;
            return false;
        }

        public Vector2 GetCurrentMovement() => _currentMovement;

        public void SetStrategy(IInputStrategy strategy)
        {
            throw new System.NotImplementedException();
        }
    }
}
