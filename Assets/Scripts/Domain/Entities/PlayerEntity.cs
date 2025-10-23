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
        public float VerticalVelocity { get; private set; }
        readonly IEventBus _eventBus;
        public AttackDataSO[] attacks;// pendiente ver que usamos de aqui o no, si hacemos un SO o no
        public float moveSpeed = 5f;
        public bool IsGrounded { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsClimbing { get; private set; }
        public bool IsHurt { get; private set; }
        public bool IsDashing{ get; private set; }
        public bool IsDead { get; private set; }

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
            _eventBus.Publish(new PlayerMovement(Id, Position));
        }

        public void TakeDamage(int amount)
        {
            var newHealth = Mathf.Max(Stats.CurrentHealth - amount, 0);
            Stats = Stats.WithHealth(newHealth);
        }

        public void Jump()
        {
            VerticalVelocity = Stats.JumpForce;
            _eventBus.Publish( new PlayerJumpStartedEvent(Id, Stats.JumpForce));
        }

        /// <summary>
        /// Aplica gravedad a la entidad jugador
        /// </summary>
        /// <param name="gravity"> se mantiene y no se usa el interno, para poder aplicar con 0 gravedad</param>
        /// <param name="deltaTime"></param>
        public void ApplyGravity(float gravity, float deltaTime)
        {
            VerticalVelocity = gravity == 0 ? 0 : VerticalVelocity - gravity * deltaTime;
            Position += deltaTime * VerticalVelocity * Vector2.up;
            _eventBus.Publish(new PlayerMovement(Id, Position));
        }

        // ToDo: temporal para testeo cuando haya fisicas y escenario
        public bool HasLanded()
        {
            // Simplificación: leer del controller o de colisiones por evento?
            return Position.y <= 0;
        }


        public void StartAttack() => IsAttacking = true;
        public void StopAttack() => IsAttacking = false;

        public void StartJump() => IsGrounded = false;
        public void StopJump() => IsGrounded = true;

        public void StartClimb() => IsClimbing = true;
        public void StopClimb() => IsClimbing = false;

        public void StartHurt() => IsHurt = true;
        public void StopHurt() => IsHurt = false;
        public void StartDash() => IsDashing = true;
        public void StopDash() => IsDashing = false;

        public void StartDead() => IsDead = true;
        public void StopDead() => IsDead = false;
    }
}
