using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
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
            _eventBus.Publish(new PlayerUpdateMoveStateView(_playerEntity.Id, StateType));
            //_eventBus.Publish(new PlayerAnimationEvent("Idle"));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd)
        {
            if (cmd is MovementCommand move && move.Direction.sqrMagnitude > 0.01f && _playerEntity.Capabilities.Has(MoveCapability.Move))
            {
                _stateMachine.ChangeState<MovingState>(MovementStateType.Moving);
            }
            else if (cmd is JumpCommand && _playerEntity.Capabilities.Has(MoveCapability.Jump))
            {
                _stateMachine.ChangeState<JumpState>( MovementStateType.Jumping ,new JumpStateContext(Vector2.zero));
            }
            else if (cmd is DashCommand && _playerEntity.Capabilities.Has(MoveCapability.Dash))
            {
                _stateMachine.ChangeState<DashState>(MovementStateType.Dash, new DashStateContext(_playerEntity.FacingDirection));
            }
        }

        public void Update(float dt) { }
    }
}
