using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class IdleState : IMovementState
    {
        readonly PlayerEntity _playerEntity;
        readonly IMovementStateMachine _stateMachine;
        readonly IEventBus _eventBus;
        public MovementStateType StateType => MovementStateType.Idle;

        public IdleState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _stateMachine = stateMachine;
            _eventBus = eventBus;
        }
        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Idle");
            //_eventBus.Publish(new PlayerAnimationEvent("Idle"));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if (cmd is MovementCommand move && move.Direction.sqrMagnitude > 0.01f)
            {
                _stateMachine.ChangeState<MovingState>(MovementStateType.Moving);
            }
            else if (cmd is JumpCommand jump)
            {
                _stateMachine.ChangeState<JumpState>( MovementStateType.Jumping ,new JumpStateContext(Vector2.zero));
            }
        }

        public void Update(float dt) { }
    }
}
