using Game.Domain.Entities.Player;
using Game.Events;
using Game.Settings;
using System;
using UnityEngine;

namespace Game.Domain.Entities
{
    /// <summary>
    /// Encapsula toda la logica de entity, todo se decide y gestiona aqui dentro
    /// </summary>
    public class PlayerEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public PlayerCapabilitySet Capabilities { get; } = new();
        public PlayerStats Stats { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 FacingDirection { get; private set; } = Vector2.right;

        public readonly MovementComponent Movement;
        public readonly CombatComponent Combat;
        public readonly HealthComponent Health;
        public readonly DamageController DamageController;
        public AttackDataSO[] attacks;// pendiente ver que usamos de aqui o no, si hacemos un SO o no

        public PlayerEntity(PlayerStats stats, IEventBus eventBus)
        {
            Stats = stats;
            Movement = new MovementComponent(this, eventBus);
            Combat = new CombatComponent(this, eventBus);
            Health = new HealthComponent(this, eventBus);
            DamageController = new DamageController();
        }

        internal void SetPosition(Vector2 newPos) => Position = newPos;

        internal void Face(Vector2 dir) => FacingDirection = dir.normalized;

        internal void UpdateHealth(PlayerStats newStats)
        {
            Stats = newStats;
        }
    }
}
