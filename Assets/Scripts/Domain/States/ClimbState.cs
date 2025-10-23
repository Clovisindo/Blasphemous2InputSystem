using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class ClimbState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;

        public ClimbState(PlayerStateMachine stateMachine, PlayerEntity playerEntity, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Climb State.");
            //event bus animacion de salto
            _playerEntity.StartClimb();
        }

        public void HandleCommand(InputCommand cmd)
        {
            if (cmd is JumpCommand jump)
            {
                _stateMachine.ChangeState<JumpState>();
            }
            else if ( cmd is AttackCommand attack)
            {
                _stateMachine.ChangeState<AttackingState>(new AttackStateContext(attack.Type));
            }
        }

        public void Update(float dt) { }

        public void Exit()
        {
            _playerEntity.StopClimb();
            //event bus animacion si es necesario
        }

       

        
    }
}
