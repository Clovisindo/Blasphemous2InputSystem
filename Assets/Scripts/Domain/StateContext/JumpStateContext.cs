using UnityEngine;

namespace Game.Domain.StateMachine
{
    public struct JumpContextData
    {
        public readonly Vector2 MoveDirecion;
        public JumpContextData(Vector2 moveDirecion) => MoveDirecion = moveDirecion;
    }
    public class JumpStateContext : IStateContext<JumpContextData>
    {
        public JumpContextData Data { get; }
        public JumpStateContext(Vector2 moveDirecion)
        {
            Data = new JumpContextData(moveDirecion);
        }
    }
}
