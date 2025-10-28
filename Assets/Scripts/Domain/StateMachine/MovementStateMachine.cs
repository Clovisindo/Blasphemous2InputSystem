using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class MovementStateMachine : IMovementStateMachine
    {
        readonly Dictionary<Type, IMovementState> _states = new();
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
        
        IMovementState _current;
        public MovementStateType CurrentStateType => _current.StateType;

        public MovementStateMachine(PlayerEntity playerEntity, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _eventBus = eventBus;
            _states[typeof(IdleState)] = new IdleState(_playerEntity, this, eventBus);
            _states[typeof(MovingState)] = new MovingState(_playerEntity, this, eventBus);
            _states[typeof(JumpState)] = new JumpState(_playerEntity, this, eventBus);
            _states[typeof(ClimbState)] = new ClimbState(_playerEntity, this, eventBus);
            _states[typeof(DashState)] = new DashState(_playerEntity, this, eventBus);
            _states[typeof(DeathState)] = new DeathState(_playerEntity, this, eventBus);
            _states[typeof(HurtState)] = new HurtState(_playerEntity, this, eventBus);

            _current = _states[typeof(IdleState)];
            _current.Enter();
        }

        public void ChangeState<T>(MovementStateType next, IStateContext context = null) where T : IMovementState
        {
            if (!CanTransitionTo(next))
            {
                Debug.Log($"No se realiza la transicion de movimiento {CurrentStateType} a {next} por que no está incluida en las reglas.");
                return;
            }

            _current.Exit();
            _current = _states[typeof(T)];
            _current.Enter(context);
        }
        public void ProcessCommand(InputCommand cmd) => _current.HandleCommand(cmd);

        public void Update(float dt) => _current.Update(dt);

        public bool CanTransitionTo(MovementStateType nextStateType)
           => MoveTransitionRules.CanTransition(_current.StateType, nextStateType);

    }
}
