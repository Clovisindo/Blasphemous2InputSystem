using Game.Input.Commands;
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

        public readonly struct DamageIntentEvent : IApplicationEvent
        {
            public Guid TargetPlayerId { get; }
            public int Damage { get; }
            public float StunDuration { get; }
            public Vector2 KnockbackDirection { get; }
            public GameObject Source { get; }

            public DamageIntentEvent(Guid targetPlayerId, int damage, float stun, Vector2 knockbackDir, GameObject source) =>
                (TargetPlayerId, Damage, StunDuration, KnockbackDirection, Source) = (targetPlayerId, damage, stun, knockbackDir, source);
        }
        public readonly struct PlayerDamageIgnored : IDomainEvent
        {
            public readonly Guid PlayerId;

            public PlayerDamageIgnored(Guid playerId)
            {
                PlayerId = playerId;
            }
        }

        public readonly struct MoveStateEndedEvent : IApplicationEvent
        {
            public readonly Guid PlayerId;
            public readonly InputCommand InputCommand;
            public MoveStateEndedEvent(Guid playerId, InputCommand inputCommand)
            {
                PlayerId = playerId;
                InputCommand = inputCommand;
            }
        }

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
            public readonly Vector2 Position;
            public PlayerMovement(Guid id, Vector2 pos)
            {
                PlayerId = id;
                Position = pos;
            }
        }

        public readonly struct PlayerDamagedEvent : IDomainEvent
        {
            public readonly Guid PlayerId;
            public readonly int Damage;
            public bool IsDead { get; }
            public PlayerDamagedEvent(Guid id, int damage, bool isDead)
            {
                PlayerId = id;
                Damage = damage;
                IsDead = isDead;
            }
        }

        public readonly struct PlayerHurtAnimStart : IApplicationEvent
        {
            public readonly Guid PlayerId;
            public PlayerHurtAnimStart(Guid id) => PlayerId = id;
        }

        public readonly struct PlayerHurtAnimEnd : IApplicationEvent
        {
            public readonly Guid PlayerId;
            public PlayerHurtAnimEnd(Guid id) => PlayerId = id;
        }

        public readonly struct PlayerDiedEvent : IDomainEvent
        {
            public readonly Guid PlayerId;
            public PlayerDiedEvent(Guid id) => PlayerId = id;
        }

        public readonly struct PlayerHealedEvent : IDomainEvent
        {
            public readonly Guid PlayerId;
            public readonly int Amount;
            public PlayerHealedEvent(Guid id, int amount)
            {
                PlayerId = id;
                Amount = amount;
            }
        }

        public readonly struct PlayerActionBlockedEvent : IDomainEvent
        {
            public readonly Guid PlayerId;
            public readonly ActionStateType ActionStateType;
            public readonly MovementStateType CurrentMoveStateType;
            public PlayerActionBlockedEvent(Guid playerId, ActionStateType actionStateType, MovementStateType currentMoveStateType)
            {
                PlayerId = playerId;
                ActionStateType = actionStateType;
                CurrentMoveStateType = currentMoveStateType;
            }
        }

        public readonly struct PlayerUpdateMoveStateView : IApplicationEvent
        {
            public readonly Guid PlayerId;
            public readonly MovementStateType MovementStateType;

            public PlayerUpdateMoveStateView(Guid playerId, MovementStateType movementStateType)
            {
                PlayerId = playerId;
                MovementStateType = movementStateType;
            }
        }

        public readonly struct PlayerUpdateActionStateView : IApplicationEvent
        {
            public readonly Guid PlayerId;
            public readonly ActionStateType ActionStateType;

            public PlayerUpdateActionStateView(Guid playerId, ActionStateType actionStateType)
            {
                PlayerId = playerId;
                ActionStateType = actionStateType;
            }
        }
    }
}
