using Game.Events;
using Game.Settings;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Domain.Entities
{
    /// <summary>
    /// Encapsula toda la logica de entity, todo se decide y gestiona aqui dentro
    /// </summary>
    public class PlayerEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public PlayerCapabilitySet Capabilities { get; } = new();
        public Vector2 Position { get;private set; }
        public Vector2 FacingDirection { get; private set; } = Vector2.right;
        public Vector2 KnockbackVelocity { get; private set; }
        public PlayerStats Stats { get; private set; }
        public float VerticalVelocity { get; private set; }
        readonly IEventBus _eventBus;
        public AttackDataSO[] attacks;// pendiente ver que usamos de aqui o no, si hacemos un SO o no
        public bool IsGrounded { get; private set; }//interno
        public bool IsAttacking { get; private set; }
        public bool IsClimbing { get; private set; }
        public bool IsHurt { get; private set; }
        public bool IsDashing{ get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInvulnerable { get; private set; }
        

        public PlayerEntity(PlayerStats stats, IEventBus eventBus)
        {
            Stats = stats;
            _eventBus = eventBus;
            IsInvulnerable = false;
        }

        public void Move (Vector2 direction, float deltaTime)
        {
            if (!Capabilities.Has(Capability.Move)) 
                return;

            float horizontalMove = direction.x;
            Position += deltaTime * horizontalMove * Stats.Speed * Vector2.right;
            if (horizontalMove != 0)
                FacingDirection = new Vector2(Mathf.Sign(horizontalMove), 0);
            _eventBus.Publish(new PlayerMovement(Id, Position));
        }

        public void TakeDamage(int newHealth)
        {
            Stats = Stats.WithHealth(newHealth);
        }

        public void SetKnockback(Vector2 direction, float force)
        {
            KnockbackVelocity = direction.normalized * force;
        }

        public void ApplyKnockback(float deltaTime)
        {
            Position += KnockbackVelocity.x * deltaTime * Vector2.right;
            _eventBus.Publish(new PlayerMovement(Id, Position));
        }

        //ToDo: llevarse estos metodos y publicar eventos al PlayerDomainService
        public void Jump()
        {
            if (!Capabilities.Has(Capability.Move))
                return;

            IsGrounded = false;
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
            HasLanded();
        }

        // ToDo: temporal para testeo cuando haya fisicas y escenario
        public void HasLanded()
        {
            // Simplificación: leer del controller o de colisiones por evento?
            if (Position.y <= 0)
                IsGrounded = true;
        }

        public void Dash(Vector2 direction, float dashSpeed, float deltaTime)
        {
            if (!Capabilities.Has(Capability.Dash))
                return;

            direction.y = 0f;
            direction.Normalize();

            Position += direction * dashSpeed * deltaTime;//ToDo: hay que rehacer todos los movimientos con interpolacion
            _eventBus.Publish(new PlayerMovement(Id, Position));
        }


        public void StartAttack() => IsAttacking = true;
        public void StopAttack() => IsAttacking = false;
        public void StartClimb() => IsClimbing = true;
        public void StopClimb() => IsClimbing = false;
        public void StartHurt()
        {
            IsHurt = true;
            IsInvulnerable = true;
        }
        public void StopHurt()
        {
            IsHurt = false;
            IsInvulnerable = false;
        }
        public void StartDash()
        {
            IsDashing = true;
            IsInvulnerable = true;

        }
        public void StopDash()
        {
            IsDashing = false;
            IsInvulnerable = false;
            //_eventBus.Publish(new PlayerDashEnded(Id));
        }
        public void StartDead() => IsDead = true;
        public void StopDead() => IsDead = false;
    }
}
