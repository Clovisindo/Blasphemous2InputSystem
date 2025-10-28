using Game.Input.Commands;
using static Utilities;

namespace Game.Domain.StateMachine
{
    public interface IActionStateMachine
    {
        ActionStateType CurrentStateType { get; }
        bool CanTransitionTo(ActionStateType next);
        void ChangeState<T>(ActionStateType next, IStateContext context = null) where T : IActionState;
        void ProcessCommand(InputCommand cmd);
        void Update(float dt);
    }

    public interface IMovementStateMachine
    {
        MovementStateType CurrentStateType { get; }
        bool CanTransitionTo(MovementStateType next);
        void ChangeState<T>(MovementStateType next, IStateContext context = null) where T : IMovementState;
        void ProcessCommand(InputCommand cmd);
        void Update(float dt);
    }
}
