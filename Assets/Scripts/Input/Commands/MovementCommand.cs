using UnityEngine;

namespace Game.Input.Commands
{
    public class MovementCommand : InputCommand
    {
        public Vector2 Direction { get; }
        public MovementCommand(Vector2 dir, float ts) : base(ts) => Direction = dir;
    }
}
