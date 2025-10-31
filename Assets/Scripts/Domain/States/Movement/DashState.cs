using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class DashState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        private StateTimer _timer;
        private Vector2 _direction;


        public MovementStateType StateType => MovementStateType.Dash;

        public DashState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Dash State.");

            if (context is IStateContext<DashContextData> dashCtx)
                _direction = dashCtx.Data.Direction;

            _timer = new StateTimer(0.2f);
            //_eventBus.Publish(new PlayerDashStarted(_entity.Id, _direction));
            _playerEntity.StartDash();
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) 
        {
            _playerEntity.Dash(_direction, _playerEntity.Stats.DashSpeed, dt);
            _timer.Update(dt);

            if (_timer.IsFinished)
            {
                _playerEntity.StopDash();
                _stateMachine.ChangeState<IdleState>(MovementStateType.Idle);
            }
        }

        public void Exit() { }
    }
}
