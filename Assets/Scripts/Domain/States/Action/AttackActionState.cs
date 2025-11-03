using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class AttackActionState : IActionState
    {
        readonly PlayerEntity _playerEntity;
        readonly IActionStateMachine _machine;
        readonly IEventBus _eventBus;
        public ActionStateType StateType => ActionStateType.Attacking;
        StateTimer _stateTimer;
        AttackType _currentAttack;
        float _attackDuration = 0.4f;//esto se cargaria de un abilitySystem y SOs

        public AttackActionState(PlayerEntity playerEntity, IActionStateMachine machine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _machine = machine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            if (context is IStateContext<AttackContextData> attackCtx)
                _currentAttack = attackCtx.Data.Type;
            else
                _currentAttack = AttackType.Light;
            Debug.Log("Enter action Attacking");
            _eventBus.Publish(new PlayerUpdateActionStateView(_playerEntity.Id, StateType));
            _stateTimer = new StateTimer(_attackDuration);
            _playerEntity.StartAttack();
            _eventBus.Publish(new PlayerAttackStarted(_playerEntity.Id, _currentAttack));
        }

        public void Exit() 
        {
            _eventBus.Publish(new PlayerAttackFinished(_playerEntity.Id));
            _playerEntity.StopAttack();
        }

        public void HandleCommand(InputCommand cmd) { }//durante ataque no procesamos otros actions inputs

        public void Update(float dt)
        {
            _stateTimer.Update(dt);
            if (_stateTimer.IsFinished)
            {
                _machine.ChangeState<IdleActionState>(ActionStateType.Idle);
            }
        }
    }
}
