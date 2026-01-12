using Game.Domain.Entities;
using Game.Domain.Entities.Player;
using Game.Events;
using Game.Input;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
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
        const float DASH_DURATION = 2f;
        readonly IInputBuffer _inputBuffer;
        InputCommand _bufferedCommand;


        public MovementStateType StateType => MovementStateType.Dash;

        public DashState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IInputBuffer inputBuffer, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _inputBuffer = inputBuffer;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Dash State.");
            _eventBus.Publish(new PlayerUpdateMoveStateView(_playerEntity.Id, StateType));

            if (context == null)
                _direction = _playerEntity.FacingDirection;
            if (context is IStateContext<DashContextData> dashCtx)
                _direction = dashCtx.Data.Direction;

            _timer = new StateTimer(DASH_DURATION);
            //_eventBus.Publish(new PlayerDashStarted(_entity.Id, _direction));
            StartDash();

        }

        public void HandleCommand(InputCommand cmd)
        {
            _inputBuffer.AddCommand(cmd);
        }

        public void Update(float dt)
        {
            HandleDash(dt);
        }

        private void HandleDash(float dt)
        {
            _playerEntity.Movement.Dash(_direction, _playerEntity.Stats.DashSpeed, dt);
            _timer.Update(dt);
            if (_timer.IsFinished)
                EndDash();
        }

        private void StartDash()
        {
            _playerEntity.Movement.StartDash();
            _playerEntity.DamageController.AddInvulnerability(InvulnerableCapability.Dash);
        }

        private void EndDash()
        {
            _playerEntity.Movement.StopDash();
            _playerEntity.DamageController.RemoveInvulnerability(InvulnerableCapability.Dash);
            _bufferedCommand = _inputBuffer.Peek();
            _eventBus.Publish(new MoveStateEndedEvent(_playerEntity.Id, _bufferedCommand));
        }

        public void Exit()
        {
            _bufferedCommand = null;
        }
    }
}
