using Game.Domain.Entities;
using Game.Events;
using Game.Input;
using Game.Input.Commands;

namespace Game.Domain.StateMachine
{
    public class PlayerStateMachine
    {
        public IMovementStateMachine Movement { get; }
        public IActionStateMachine Action { get; }

        public PlayerStateMachine(PlayerEntity playerEntity, InputBuffer inputbuffer,IEventBus eventBus)
        {
            Movement = new MovementStateMachine(playerEntity, inputbuffer, eventBus);
            Action = new ActionStateMachine(playerEntity, eventBus);
        }

        public void ProcessCommand(InputCommand cmd) { }

        public void Update(float dt)
        {
            Movement.Update(dt);
            Action.Update(dt);
        }

        public void ExecuteCombo(ComboType type)
        {
            // ejemplo: pasar a estado combo/ataque potente según combo
            // _current = _states[typeof(AttackingState)]; ...
        }
    }
}
