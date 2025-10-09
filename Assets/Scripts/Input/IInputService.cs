using Game.Input.Commands;

namespace Game.Input
{
    public interface IInputService 
    {
        void Initialize(PlayerInputActions actions);
        bool TryDequeue(out InputCommand command);
        void Enqueue(InputCommand command);
        void SetStrategy(IInputStrategy strategy);
    }
}