using UnityEngine;
using static Utilities;

namespace Game.Events
{
    public interface IEvent { }
    public struct PlayerMoveEvent : IEvent
    {
        public Vector2 MovementDelta;
    }
    public struct PlayerAttackEvent : IEvent
    {
        public AttackType Type;
    }
    public struct PlayerAnimationEvent : IEvent
    {
        public string TriggerName;
    }
}
