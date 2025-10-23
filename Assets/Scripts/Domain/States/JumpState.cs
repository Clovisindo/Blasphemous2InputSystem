using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class JumpState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;

        bool _isFalling;

        public JumpState(PlayerEntity playerEntity, PlayerStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Jump State.");
            _isFalling = false;
            _playerEntity.StartJump();
            _playerEntity.Jump();
            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.JumpStart);
        }

        public void Update(float dt)
        {
            HandleJump(dt);
        }


        public void HandleCommand(InputCommand cmd)
        {
            // No procesamos más comandos durante el salto,
            // pero podríamos meterlos en el inputBuffer si quisieramos
            //o permitir atacar
        }

        private void HandleJump(float dt)
        {
            _playerEntity.ApplyGravity(_playerEntity.Stats.Gravity, dt);
            if (_playerEntity.VerticalVelocity < 0 && !_isFalling)
            {
                _isFalling = true;
                //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.FallStart));
            }
            if( _playerEntity.HasLanded())
            {
                _playerEntity.ApplyGravity(0, dt);
                _playerEntity.StopJump();
                _stateMachine.ChangeState<IdleState>();
                //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.Land));
            }
        }

        public void Exit()
        {
            //_eventBus.Publish(new PlayerAnimationEvent(PlayerAnimationType.JumpEnd));
        }


       
    }
}
