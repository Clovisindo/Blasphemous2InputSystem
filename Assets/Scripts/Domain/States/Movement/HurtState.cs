using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class HurtState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        private StateTimer _timer;
        public MovementStateType StateType => MovementStateType.Hurt;

        public HurtState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter hurt State.");
            _eventBus.Publish(new PlayerHurtStartedEvent(_playerEntity.Id));
            _timer = new StateTimer(0.5f);
            _playerEntity.StartHurt();
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt)
        {
            _timer.Update(dt);
            if(_timer.IsFinished)
            {
                _playerEntity.StopHurt();
                _eventBus.Publish(new PlayerHurtEndedEvent(_playerEntity.Id));
                _stateMachine.ChangeState<IdleState>(MovementStateType.Idle);
            }
        }

        public void Exit() { }
    }
}
