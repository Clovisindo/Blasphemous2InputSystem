using Game.Events;
using Game.Input.Commands;
using Game.Settings;
using System;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class IdleState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerSettingsSO _settings;
        readonly IEventBus _eventBus;

        public IdleState(PlayerStateMachine stateMachine,PlayerSettingsSO settings, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _settings = settings;
            _eventBus = eventBus;
        }
        public void Enter()
        {
            Debug.Log("Enter Idle");
            //_eventBus.Publish(new PlayerAnimationEvent("Idle"));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if (cmd is MovementCommand move && move.Direction.sqrMagnitude > 0.01f)
            {
                _stateMachine.ChangeState<MovingState>();
            }
            else if (cmd is AttackCommand atk)
            {
                _stateMachine.ChangeState<AttackingState>();
                _eventBus.Publish(new PlayerAttackEvent{ Type = atk.Type });
            }
        }

        public void Update(float dt) { }
    }
}
