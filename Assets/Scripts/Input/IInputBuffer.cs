using Game.Input.Commands;

namespace Game.Input
{
    public interface IInputBuffer
    {
        void AddCommand(InputCommand command);
        void Clear();
        void Consume();
        ComboType DetectCombo();
        bool HasRecentCommand();
        bool LastIs<T>() where T : InputCommand;
        InputCommand Peek();
        InputCommand PeekFirst();
        bool TryDequeue(out InputCommand command);
    }
}