
using System;
using UnityEngine;

namespace Game.Events.PlayerEvents
{
    public static class PlayerEvents
    {
        public struct CoreReadyEvent 
        {
            public readonly Guid PlayerId;
            public CoreReadyEvent(Guid id)
            {
                PlayerId = id;
            }
        };
        public struct AttackStarted
        {
            public readonly Guid PlayerId;
            public AttackStarted(Guid id) => PlayerId = id;
        }

        public struct AttackFinished
        {
            public readonly Guid PlayerId;
            public AttackFinished(Guid id) => PlayerId = id;
        }

        public struct Movement
        {
            public readonly Guid PlayerId;
            public readonly Vector2 Direction;
            public Movement(Guid id, Vector2 dir)
            {
                PlayerId = id;
                Direction = dir;
            }
        }
    }
}
