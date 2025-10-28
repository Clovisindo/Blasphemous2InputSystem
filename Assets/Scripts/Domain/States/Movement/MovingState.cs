using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class MovingState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        Vector2 _lasMoveDirection;// pasar info a JumpState del ultimo movimiento
        public MovementStateType StateType => MovementStateType.Moving;

        public MovingState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _stateMachine = stateMachine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Moving state.");
            //_eventBus.Publish(new PlayerStartedMovingEvent(_entity.Id));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if ( cmd is MovementCommand move)
            {
                HandleMovement(move);
            }
            else if (cmd is JumpCommand jump)
            {
                _stateMachine.ChangeState<JumpState>( MovementStateType.Jumping, new JumpStateContext(_lasMoveDirection));
            }
        }

        void HandleMovement( MovementCommand move)
        {
            _lasMoveDirection = move.Direction;
            if (move.Direction.sqrMagnitude > 0.01f)
                _playerEntity.Move(move.Direction, move.Timestamp);
            else
            {
                //_eventBus.Publish(new PlayerStoppedMovingEvent(_entity.Id));
                _stateMachine.ChangeState<IdleState>(MovementStateType.Idle);
            }
        }

        /// <summary>
        /// solo par animaciones dependientes del tiempo
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt) { }
    }
}
