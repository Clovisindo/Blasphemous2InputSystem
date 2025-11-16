using Game.Input.Commands;

namespace Game.Domain.StateMachine
{
    public interface IPlayerState
    {
        void Enter(IStateContext context = null);
        void Exit();
        void HandleCommand(InputCommand cmd);
        void Update(float dt);
    }
}
