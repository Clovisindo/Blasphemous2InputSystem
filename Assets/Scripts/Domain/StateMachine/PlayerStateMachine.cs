using Game.Domain.Entities;
using Game.Events;
using Game.Input;
using Game.Input.Commands;
using System;
using System.Collections.Generic;

namespace Game.Domain.StateMachine
{
    public class PlayerStateMachine
    {
        readonly Dictionary<Type, IPlayerState> _states = new();
        readonly PlayerEntity _playerEntity;
        IPlayerState _current;

        public PlayerStateMachine(PlayerEntity playerEntity,IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _states[typeof(IdleState)] = new IdleState(playerEntity, this, eventBus);
            _states[typeof(MovingState)] = new MovingState(playerEntity, this, eventBus);
            _states[typeof(AttackingState)] = new AttackingState(playerEntity, this, eventBus);
            
            _current = _states[typeof(IdleState)];
            _current.Enter();
        }

        public void ChangeState<T>(IStateContext context = null) where T : IPlayerState
        {
            _current.Exit();
            _current = _states[typeof(T)];
            _current.Enter(context);
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
