
using System;
using UnityEngine;
using static Utilities;

namespace Game.Events.PlayerEvents
{
    public static class PlayerEvents
    {
        public readonly struct CoreReadyEvent : IApplicationEvent
        {
            public readonly Guid PlayerId;
            public CoreReadyEvent(Guid id)
            {
                PlayerId = id;
            }
        };
        public readonly struct PlayerAttackStarted :IDomainEvent
        {
            public readonly Guid PlayerId;
            public readonly AttackType AttackType;

            public PlayerAttackStarted(Guid playerId, AttackType attackType)
            {
                PlayerId = playerId;
                AttackType = attackType;
            }
        }

        public readonly struct PlayerAttackFinished : IDomainEvent
        {
            public readonly Guid PlayerId;
            public PlayerAttackFinished(Guid id) => PlayerId = id;
        }

        public readonly struct PlayerJumpStartedEvent : IDomainEvent
        {
            public readonly Guid PlayerId;
            public readonly float JumpForce;
            public PlayerJumpStartedEvent(Guid id, float jumpForce)
            {
                PlayerId = id;
                JumpForce = jumpForce;
            }
        }

        public readonly struct PlayerMovement : IDomainEvent
        {
            public readonly Guid PlayerId;
            public readonly Vector2 Direction;
            public PlayerMovement(Guid id, Vector2 dir)
            {
                PlayerId = id;
                Direction = dir;
            }
        }
    }
}
