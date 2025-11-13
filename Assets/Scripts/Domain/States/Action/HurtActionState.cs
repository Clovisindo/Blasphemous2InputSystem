using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class HurtActionState : IActionState
    {
        readonly PlayerEntity _playerEntity;
        readonly IActionStateMachine _stateMachine;
        readonly IEventBus _eventBus;
        private StateTimer _timer;
        private const float KNOCKBACK_RESISTENCE = 0.9f;
        private const float CANCEL_WINDOW = 1.8f;
        public ActionStateType StateType => ActionStateType.Hurt;

        public HurtActionState(PlayerEntity playerEntity, IActionStateMachine stateMachine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _stateMachine = stateMachine;
            _eventBus = eventBus;
        }
        //Aqui podría ir logica de cancelacion de animaciones de ataque, o gestionar parrys, inmunidades o casos especiales
        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter hurt action State.");
            _eventBus.Publish(new PlayerUpdateActionStateView(_playerEntity.Id, StateType));
            DisableMovement();
            _timer = new StateTimer(2f);
            _playerEntity.Health.StartHurt(_playerEntity.Id);
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt)
        {
            _timer.Update(dt);
            _playerEntity.Movement.ApplyKnockback(dt);
            _playerEntity.Movement.SetKnockback(_playerEntity.Movement.KnockbackVelocity * KNOCKBACK_RESISTENCE, _playerEntity.Stats.KnockbackForce);

            if (_timer.Elapsed >= CANCEL_WINDOW)
                EnableMovement();

            if (_timer.IsFinished)
            {
                _playerEntity.Health.StopHurt(_playerEntity.Id);
                _stateMachine.ChangeState<IdleActionState>(ActionStateType.Idle);
            }
        }

        public void Exit()
        {
            EnableMovement();
            _playerEntity.Capabilities.Enable(Capability.Hurt);
        }

        private void DisableMovement()
        {
            _playerEntity.Capabilities.Disable(Capability.Move);
            _playerEntity.Capabilities.Disable(Capability.Jump);
            _playerEntity.Capabilities.Disable(Capability.Dash);
            _playerEntity.Capabilities.Disable(Capability.Hurt);
        }

        private void EnableMovement()
        {
            _playerEntity.Capabilities.Enable(Capability.Move);
            _playerEntity.Capabilities.Enable(Capability.Jump);
            _playerEntity.Capabilities.Enable(Capability.Dash);
            
        }
    }
}
