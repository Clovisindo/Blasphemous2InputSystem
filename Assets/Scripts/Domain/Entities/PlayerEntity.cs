using Game.Events;
using Game.Settings;
using System;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Domain.Entities
{
    /// <summary>
    /// Encapsula toda la logica de entity, todo se decide y gestiona aqui dentro
    /// </summary>
    public class PlayerEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Vector2 Position { get;private set; }
        public Vector2 FacingDirection { get; private set; } = Vector2.right;
        public PlayerStats Stats { get; private set; }
        readonly IEventBus _eventBus;
        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public AttackDataSO[] attacks;// pendiente ver que usamos de aqui o no, si hacemos un SO o no

        public bool IsGrounded { get; private set; }
        public bool IsAttacking { get; private set; }

        public PlayerEntity(PlayerStats stats, IEventBus eventBus)
        {
            Stats = stats;
            _eventBus = eventBus;
        }

        public void Move (Vector2 direction, float deltaTime)
        {
            Position += direction * Stats.Speed * deltaTime;
            if (direction.x != 0)
                FacingDirection = new Vector2(Mathf.Sign(direction.x), 0);
            _eventBus.Publish(new Movement(Id, Position));
        }
        public void TakeDamage(int amount)
        {
            var newHealth = Mathf.Max(Stats.CurrentHealth - amount, 0);
            Stats = Stats.WithHealth(newHealth);
        }
        public void StartAttack() => IsAttacking = true;
        public void StopAttack() => IsAttacking = false;
    }
}
