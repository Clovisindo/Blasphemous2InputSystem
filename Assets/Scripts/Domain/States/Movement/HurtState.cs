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
        private const float KNOCKBACK_RESISTENCE = 0.9f;
        public MovementStateType StateType => MovementStateType.Hurt;

        public HurtState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null) 
        {
            _eventBus.Publish(new PlayerUpdateMoveStateView(_playerEntity.Id, StateType));
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }

        public void Exit() { }
    }
}
