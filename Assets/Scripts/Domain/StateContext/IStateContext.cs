namespace Game.Domain.StateMachine
{
    public interface IStateContext
    {
    }
    public interface IStateContext<T> : IStateContext
    {
        T Data { get; }
    }
}
