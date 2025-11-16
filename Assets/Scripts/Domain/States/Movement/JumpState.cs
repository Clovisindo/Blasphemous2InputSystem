using Game.Domain.Entities;
using Game.Events;
using Game.Input;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class JumpState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        Vector2 _initialDir;
        readonly InputBuffer _inputBuffer;
        InputCommand _bufferedCommand;
        public MovementStateType StateType => MovementStateType.Jumping;

        bool _isFalling;

        public JumpState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, InputBuffer inputBuffer, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _inputBuffer = inputBuffer;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Jump State.");
            _eventBus.Publish(new PlayerUpdateMoveStateView(_playerEntity.Id, StateType));

            if (context is IStateContext<JumpContextData> jumpCtx)
                _initialDir = jumpCtx.Data.MoveDirecion;
            StartJump();

            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.JumpStart);
        }

        public void Update(float dt)
        {
            HandleJump(dt);
        }

        public void HandleCommand(InputCommand cmd)
        {
            
            if (cmd is MovementCommand move)
                _initialDir = move.Direction;
            else
                _inputBuffer.AddCommand(cmd);//buffer al salir estado, no registramos moveCommands
        }

        private void HandleJump(float dt)
        {
            _playerEntity.Movement.ApplyGravity(_playerEntity.Stats.Gravity, dt);
            if (_initialDir != Vector2.zero)//si hubo antes o en jumpstate algun comando de movimiento,actualizamos la posicion
                _playerEntity.Movement.Move(_initialDir, dt);

            if (_playerEntity.Movement.VerticalVelocity < 0 && !_isFalling)
                StartFalling();

            if (_playerEntity.Capabilities.Has(MoveCapability.IsGrounded))
                EndJump(dt);
        }

        private void StartJump()
        {
            _isFalling = false;
            _playerEntity.Movement.Jump();
        }

        private void StartFalling()
        {
            _isFalling = true;
            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.FallStart));
        }

        private void EndJump(float dt)
        {
            _playerEntity.Movement.ApplyGravity(0, dt);

            _bufferedCommand = _inputBuffer.Peek();

            _eventBus.Publish(new MoveStateEndedEvent(_playerEntity.Id, _bufferedCommand));
            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.Land));
        }

        public void Exit() 
        {
            _bufferedCommand = null;
        }
    }
}
