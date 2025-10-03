using static Utilities;

namespace Game.Input.Commands
{
   
    public class AttackCommand : InputCommand
    {
        public AttackType Type { get; }
        public AttackCommand(AttackType t, float ts) : base(ts) => Type = t;
    }
}
