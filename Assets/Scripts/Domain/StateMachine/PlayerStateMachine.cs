using Game.Events;
using Game.Input;
using Game.Input.Commands;
using Game.Settings;
using System;
using System.Collections.Generic;

namespace Game.Domain.StateMachine
{
    public class PlayerStateMachine
    {
        readonly Dictionary<Type, IPlayerState> _states = new();
        IPlayerState _current;

        public PlayerStateMachine(PlayerSettingsSO settings,IEventBus eventBus)
        {
            _states[typeof(IdleState)] = new IdleState(this,settings,eventBus);
            _states[typeof(MovingState)] = new MovingState(this, settings, eventBus);
            _states[typeof(AttackingState)] = new AttackingState(this, settings, eventBus);
            
            _current = _states[typeof(IdleState)];
            _current.Enter();
        }

        public void ChangeState<T>() where T : IPlayerState
        {
            _current.Exit();
            _current = _states[typeof(T)];
            _current.Enter();
        }

        public void ProcessCommand(InputCommand cmd) => _current.HandleCommand(cmd);

        public void Update(float dt) => _current.Update(dt);

        public void ExecuteCombo(ComboType type)
        {
            // ejemplo: pasar a estado combo/ataque potente según combo
            // _current = _states[typeof(AttackingState)]; ...
        }
    }
}
