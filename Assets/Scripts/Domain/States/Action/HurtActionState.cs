using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
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
            _eventBus.Publish(new PlayerHurtAnimStart(_playerEntity.Id));
            _timer = new StateTimer(2f);
            _playerEntity.StartHurt();
        }

       

        public void Exit() 
        {
            EnableMovement();
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt)
        {
            _timer.Update(dt);
            _playerEntity.ApplyKnockback(dt);
            _playerEntity.SetKnockback(_playerEntity.KnockbackVelocity * KNOCKBACK_RESISTENCE, _playerEntity.Stats.KnockbackForce);

            if (_timer.IsFinished)
            {
                _playerEntity.StopHurt();
                _eventBus.Publish(new PlayerHurtAnimEnd(_playerEntity.Id));
                _stateMachine.ChangeState<IdleActionState>(ActionStateType.Idle);
            }
        }

        private void DisableMovement()
        {
            _playerEntity.Capabilities.Disable(Capability.Move);
            _playerEntity.Capabilities.Disable(Capability.Jump);
            _playerEntity.Capabilities.Disable(Capability.Dash);
        }

        private void EnableMovement()
        {
            _playerEntity.Capabilities.Enable(Capability.Move);
            _playerEntity.Capabilities.Enable(Capability.Jump);
            _playerEntity.Capabilities.Enable(Capability.Dash);
        }
    }
}
