using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class JumpState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        Vector2 _initialDir;
        public MovementStateType StateType => MovementStateType.Jumping;

        bool _isFalling;

        public JumpState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Jump State.");
            if (context is IStateContext<JumpContextData> jumpCtx)
                _initialDir = jumpCtx.Data.MoveDirecion;
            _isFalling = false;
            _playerEntity.Jump();
            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.JumpStart);
        }

        public void Update(float dt)
        {
            HandleJump(dt);
        }


        public void HandleCommand(InputCommand cmd)
        {
            if (cmd is MovementCommand move)
            {
                _initialDir = move.Direction;
            }
        }

        private void HandleJump(float dt)
        {
            _playerEntity.ApplyGravity(_playerEntity.Stats.Gravity, dt);

            if (_initialDir != Vector2.zero)//si hubo antes o en jumpstate algun comando de movimiento,actualizamos la posicion
                _playerEntity.Move(_initialDir, dt);

            if (_playerEntity.VerticalVelocity < 0 && !_isFalling)
            {
                _isFalling = true;
                //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.FallStart));
            }
            if( _playerEntity.IsGrounded)
            {
                _playerEntity.ApplyGravity(0, dt);
                _stateMachine.ChangeState<IdleState>(MovementStateType.Idle);
                //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.Land));
            }
        }

        public void Exit()
        {
            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.JumpEnd));
        }


       
    }
}
