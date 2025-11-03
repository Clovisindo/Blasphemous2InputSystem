using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class DashActionState : IActionState
    {
        readonly PlayerEntity _playerEntity;
        readonly IActionStateMachine _machine;
        readonly IEventBus _eventBus;
        public ActionStateType StateType => ActionStateType.Dashing;

        public DashActionState(PlayerEntity playerEntity, IActionStateMachine machine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _machine = machine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            _eventBus.Publish(new PlayerUpdateActionStateView(_playerEntity.Id, StateType));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }
    }
}
