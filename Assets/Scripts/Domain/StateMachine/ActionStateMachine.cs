using Game.Domain.Entities;
using Game.Events;
using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public class ActionStateMachine : IActionStateMachine
    {
        readonly Dictionary<Type, IActionState> _states = new();
        readonly PlayerEntity _playerEntity;
        readonly IEventBus _eventBus;
       
        IActionState _current;
        public ActionStateType CurrentStateType => _current.StateType;


        public ActionStateMachine(PlayerEntity playerEntity, IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _eventBus = eventBus;
            _states[typeof(IdleActionState)] = new IdleActionState(_playerEntity, this, _eventBus);
            _states[typeof(AttackActionState)] = new AttackActionState(_playerEntity, this, _eventBus);
            _states[typeof(HurtActionState)] = new HurtActionState(_playerEntity, this, _eventBus);
            _states[typeof(DashActionState)] = new DashActionState(_playerEntity, this, _eventBus);
            _states[typeof(DeathActionState)] = new DeathActionState(_playerEntity, this, _eventBus);


            _current = _states[typeof(IdleActionState)];
            _current.Enter();
        }

        public void ChangeState<T>(ActionStateType next, IStateContext context = null) where T : IActionState
        {
            if (!CanTransitionTo(next))
            {
                Debug.Log($"No se realiza la transicion de action {CurrentStateType} a {next} por que no está incluida en las reglas.");
                return; 
            }

            _current.Exit();
            _current = _states[typeof(T)];
            _current.Enter(context);
        }

        public void ProcessCommand(InputCommand cmd) => _current.HandleCommand(cmd);

        public void Update(float dt) => _current.Update(dt);

        public bool CanTransitionTo(ActionStateType nextStateType) 
            => ActionTransitionRules.CanTransition(_current.StateType, nextStateType);

    }
}
