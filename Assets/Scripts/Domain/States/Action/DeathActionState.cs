using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using static Game.Events.PlayerEvents.PlayerEvents;
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
        //Aqui podría ir logica de cancelacion de animaciones de ataque, o gestionar parrys, inmunidades o casos especiales
        public void Enter(IStateContext context = null)
        {
            _eventBus.Publish(new PlayerUpdateActionStateView(_playerEntity.Id, StateType));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }
    }
}
