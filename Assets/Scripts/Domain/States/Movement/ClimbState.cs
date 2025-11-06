using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class ClimbState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        public MovementStateType StateType => MovementStateType.Climb;

        public ClimbState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Climb State.");
            _eventBus.Publish(new PlayerUpdateMoveStateView(_playerEntity.Id, StateType));
            //event bus animacion de salto
            //_playerEntity.StartClimb();
        }

        public void HandleCommand(InputCommand cmd)
        {
            if (cmd is JumpCommand jump)
            {
                _stateMachine.ChangeState<JumpState>(MovementStateType.Jumping);
            }
        }

        public void Update(float dt) { }

        public void Exit()
        {
            //_playerEntity.StopClimb();
            //event bus animacion si es necesario
        }
    }
}
