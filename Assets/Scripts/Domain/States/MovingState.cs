using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class MovingState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;

        public MovingState(PlayerEntity playerEntity, PlayerStateMachine stateMachine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _stateMachine = stateMachine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Moving");
            //_eventBus.Publish(new PlayerAnimationEvent("Run"));
            //_eventBus.Publish(new PlayerStartedMovingEvent(_entity.Id));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if ( cmd is MovementCommand move)
            {
                HandleMovement(move);
            }
            else if ( cmd is AttackCommand atk)
            {
                _stateMachine.ChangeState<AttackingState>(new AttackStateContext(atk.Type));
            }
        }

        void HandleMovement( MovementCommand move)
        {
            if (move.Direction.sqrMagnitude > 0.01f)
                _playerEntity.Move(move.Direction, move.Timestamp);
            else
            {
                //_eventBus.Publish(new PlayerStoppedMovingEvent(_entity.Id));
                _stateMachine.ChangeState<IdleState>();
            }
        }

        /// <summary>
        /// solo par animaciones dependientes del tiempo
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt) { }
    }
}
