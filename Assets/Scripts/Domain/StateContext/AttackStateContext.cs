using static Utilities;

namespace Game.Domain.StateMachine
{
    public struct AttackContextData
    {
        public readonly AttackType Type;

        public AttackContextData(AttackType type)
        {
            Type = type;
        }
    }
    public class AttackStateContext : IStateContext<AttackContextData>
    {
        public AttackContextData Data { get; }

        public AttackStateContext(AttackType type)
        {
            Data = new AttackContextData(type);
        }
    }
}
