using Game.Input;
using Game.Input.Commands;

namespace Game.Domain.StateMachine
{
    public interface IPlayerStateMachine
    {
        IActionStateMachine Action { get; }
        IMovementStateMachine Movement { get; }

        void ExecuteCombo(ComboType type);
        void ProcessCommand(InputCommand cmd);
        void Update(float dt);
    }
}