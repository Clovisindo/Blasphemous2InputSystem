using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using UnityEngine;

namespace Game.Domain.StateMachine
{
    public class DeathState : IPlayerState
    {
        readonly PlayerEntity _playerEntity;
        readonly PlayerStateMachine _machine;
        readonly IEventBus _eventBus;

        public DeathState(PlayerEntity playerEntity, PlayerStateMachine machine, IEventBus eventBus)
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
