using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class HurtState : IMovementState
    {
        readonly IMovementStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        public MovementStateType StateType => MovementStateType.Hurt;

        public HurtState(PlayerEntity playerEntity, IMovementStateMachine stateMachine, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter hurt State.");
            //event bus animacion de daño
            _playerEntity.StartHurt();
            //ToDo: hay que gestionar como hacer invulnerable este rato
        }

        public void Exit()
        {
            //event bus fin animacion de daño??
            _playerEntity.StopHurt();
            _stateMachine.ChangeState<IdleState>(MovementStateType.Idle);
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }
    }
}
