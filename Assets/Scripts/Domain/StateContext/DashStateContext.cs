using UnityEngine;

namespace Game.Domain.StateMachine
{
    public struct DashContextData
    {
        public readonly Vector2 Direction;
        public DashContextData(Vector2 direction) => Direction = direction;
    }
    public class DashStateContext : IStateContext<DashContextData>
    {
        public DashContextData Data { get; }
        public DashStateContext(Vector2 direction)
        {
            Data = new DashContextData(direction);
        }
    }
}
