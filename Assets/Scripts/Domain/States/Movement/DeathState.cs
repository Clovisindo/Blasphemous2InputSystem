using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class DeathState : IMovementState
    {
        readonly PlayerEntity _playerEntity;
        readonly IMovementStateMachine _machine;
        readonly IEventBus _eventBus;
        public MovementStateType StateType => MovementStateType.Death;

        public DeathState(PlayerEntity playerEntity, IMovementStateMachine machine, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _machine = machine;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Attacking");
            _playerEntity.StartDead();
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }

        public void Exit()
        {
            //evento fin de juego al acabar animacion
        }
    }
}
