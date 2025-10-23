using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class DashState : IPlayerState
    {
        readonly PlayerStateMachine _stateMachine;
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;

        public DashState(PlayerStateMachine stateMachine, PlayerEntity playerEntity, IEventBus eventBus)
        {
            _stateMachine = stateMachine;
            _playerEntity = playerEntity;
            _eventBus = eventBus;
        }

        public void Enter(IStateContext context = null)
        {
            Debug.Log("Enter Dash State.");
            //event bus animacion de dash
            _playerEntity.StartDash();//como gestionamos el movimiento?
        }

        public void HandleCommand(InputCommand cmd) { }

        public void Update(float dt) { }

        public void Exit()
        {
            //event bus fin animacion de dash ??
            _playerEntity.StopDash();
            _stateMachine.ChangeState<IdleState>();

        }
    }
}
