using Game.Input.Commands;

namespace Game.Domain.StateMachine
{
    public interface IPlayerState
    {
        void Enter();
        void Exit();
        void HandleCommand(InputCommand cmd);
        void Update(float dt);
    }
}
