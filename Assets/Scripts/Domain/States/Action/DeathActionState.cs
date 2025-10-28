using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class DeathActionState : IActionState
    {
        readonly PlayerEntity _playerEntity;
        readonly IActionStateMachine _machine;
        readonly IEventBus _eventBus;
        public ActionStateType StateType => ActionStateType.Death;

        public DeathActionState(PlayerEntity playerEntity, IActionStateMachine machine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _machine = machine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }
    }
}
