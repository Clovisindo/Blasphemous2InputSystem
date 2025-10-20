using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class AttackingState : IPlayerState
    {
        readonly PlayerEntity _playerEntity;
        readonly PlayerStateMachine _machine;
        readonly IEventBus _eventBus;
        AttackType _currentAttack;
        float _attackDuration = 0.4f;//esto se cargaria de un abilitySystem y SOs
        float _timer;

        public AttackingState(PlayerEntity playerEntity, PlayerStateMachine machine, IEventBus eventBus)
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
            Debug.Log("Enter Attacking");
            _timer = _attackDuration;
            _playerEntity.StartAttack();
            _eventBus.Publish(new PlayerAttackStarted(_playerEntity.Id, _currentAttack));
        }

        public void Exit() 
        {
            _eventBus.Publish(new PlayerAttackFinished(_playerEntity.Id));
            _playerEntity.StopAttack();
        }

        public void HandleCommand(InputCommand cmd)
        {
            // durante ataque no procesamos movimiento
        }

        public void Update(float dt)
        {
            _timer -= dt;
            if (_timer <= 0f)
            {
                _machine.ChangeState<IdleState>();
            }
        }
    }
}
