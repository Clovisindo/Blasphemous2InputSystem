using Game.Input.Commands;

namespace Game.Input
{
    public interface IInputService 
    {
        void Initialize();
        bool TryDequeue(out InputCommand command);
        void Enqueue(InputCommand command);
    }
}