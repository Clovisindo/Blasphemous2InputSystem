using Game.Domain.Entities;
using Game.Input.Commands;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class AttackingState : IPlayerState
    {
        readonly PlayerEntity _playerEntity;
        readonly PlayerStateMachine _machine;
        float _attackDuration = 0.4f;//esto se cargaria de un abilitySystem y SOs
        float _timer;

        public AttackingState(PlayerEntity playerEntity, PlayerStateMachine machine)
        {
            _playerEntity = playerEntity;
            _machine = machine;
        }

        public void Enter()
        {
            Debug.Log("Enter Attacking");
            _timer = _attackDuration;
            _playerEntity.StartAttack();
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            // durante ataque no procesamos movimiento
        }

        public void Update(float dt)
        {
            _timer -= dt;
            if (_timer <= 0f)
            {
                _playerEntity.StopAttack();
                _machine.ChangeState<IdleState>();
            }
        }
    }
}
