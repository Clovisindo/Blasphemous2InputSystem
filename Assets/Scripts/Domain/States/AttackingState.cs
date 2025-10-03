using Game.Events;
using Game.Input.Commands;
using Game.Settings;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class AttackingState : IPlayerState
    {
        readonly PlayerStateMachine _machine;
        readonly PlayerSettingsSO _settings;
        readonly IEventBus _bus;
        float _attackDuration = 0.4f;//esto se cargaria de un abilitySystem y SOs
        float _timer;

        public AttackingState(PlayerStateMachine machine, PlayerSettingsSO settings, IEventBus bus)
        {
            _machine = machine;
            _settings = settings;
            _bus = bus;
        }

        public void Enter()
        {
            Debug.Log("Enter Attacking");
            _timer = _attackDuration;
            //_bus.Publish(new PlayerAnimationEvent("Attack"));
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
                _machine.ChangeState<IdleState>();
            }
        }
    }
}
