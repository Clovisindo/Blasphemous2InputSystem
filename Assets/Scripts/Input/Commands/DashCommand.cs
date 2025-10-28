using UnityEngine;

namespace Game.Input.Commands
{
    public class DashCommand : InputCommand
    {
        public float Power { get; }
        public Vector2 Direction { get; }
        public DashCommand(float power, Vector2 direction, float ts) : base(ts)
        {
            Power = power;
            Direction = direction;
        }
    }
}
