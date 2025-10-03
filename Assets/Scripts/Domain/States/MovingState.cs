using Game.Events;
using Game.Input.Commands;
using Game.Settings;
using System;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class MovingState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerSettingsSO _settings;
        readonly IEventBus _eventBus;
        Vector2 _lastInput;

        public MovingState(PlayerStateMachine stateMachine, PlayerSettingsSO settings, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _settings = settings;
            _eventBus = eventBus;
        }

        public void Enter()
        {
            Debug.Log("Enter Moving");
            //_eventBus.Publish(new PlayerAnimationEvent("Run"));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if ( cmd is MovementCommand move)
            {
                Debug.Log($"MovementCommand vector move : {move.Direction}");
                _lastInput = move.Direction;
                if(_lastInput.sqrMagnitude < 0.01f)
                    _stateMachine.ChangeState<IdleState>();
            }
            else if ( cmd is AttackCommand atk)
            {
                _stateMachine.ChangeState<AttackingState>();
                _eventBus.Publish(new PlayerAttackEvent { Type = atk.Type });
            }
        }

        public void Update(float dt)
        {
            if (_lastInput.sqrMagnitude > 0.01f)
            {
                var movement = _lastInput.normalized * _settings.moveSpeed * dt;
                _eventBus.Publish(new PlayerMoveEvent { MovementDelta = movement });
            }
        }
    }
}
