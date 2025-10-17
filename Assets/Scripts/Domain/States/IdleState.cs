using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using Game.Settings;
using System;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class IdleState : IPlayerState
    {
        readonly PlayerEntity _playerEntity;
        readonly PlayerStateMachine _stateMachine;

        public IdleState(PlayerEntity playerEntity, PlayerStateMachine stateMachine)
        {
            _playerEntity = playerEntity;
            _stateMachine = stateMachine;
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
            }
        }

        public void Update(float dt) { }
    }
}
