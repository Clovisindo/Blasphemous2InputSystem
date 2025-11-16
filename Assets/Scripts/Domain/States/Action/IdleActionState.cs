using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class IdleActionState : IActionState
    {
        readonly PlayerEntity _playerEntity;
        readonly IActionStateMachine _machine;
        readonly IEventBus _eventBus;
        public ActionStateType StateType => ActionStateType.Idle;

        public IdleActionState(PlayerEntity playerEntity, IActionStateMachine machine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _machine = machine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null) 
        {
            Debug.Log("Enter Action idle.");
            _eventBus.Publish(new PlayerUpdateActionStateView(_playerEntity.Id, StateType));
        }

        public void Exit() { }

        public void HandleCommand(InputCommand cmd) 
        {
            if (cmd is AttackCommand attack && _machine.CanTransitionTo(ActionStateType.Attacking))
            {
                _machine.ChangeState<AttackActionState>(
                    ActionStateType.Attacking,
                    new AttackStateContext(attack.Type)
                );
            }
            else// ToDo: temporal
                Debug.Log($" La accion  de atacar está bloqueada con el estado actual de la maquina de estados de accion: {_machine.CurrentStateType}");
        }

        public void Update(float dt) { }
    }
}
