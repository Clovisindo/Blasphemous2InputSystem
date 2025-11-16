using Game.Domain.Entities;
using Game.Domain.StateMachine;
using Game.Events;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.Services
{
    /// <summary>
    /// Sirve para operaciones que afectan a varias entidades o involucran lógica compartida (ej. aplicar daño, resolver knockback…).
    /// </summary>
    public class PlayerDomainService
    {
        readonly IEventBus _eventBus;

        public PlayerDomainService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void ApplyDamage(PlayerEntity player, MovementStateType currentMoveStateType, int damage, Vector2 knockbackDir)
        {
            if (!PlayerStateRules.CanCombine(currentMoveStateType, ActionStateType.Hurt))
            {
                BlockAction(player, ActionStateType.Hurt, currentMoveStateType);
                return;
            }
            HandleDamage(player, currentMoveStateType, damage, knockbackDir);
        }
        private void HandleDamage(PlayerEntity player, MovementStateType currentMoveStateType, int damage, Vector2 knockbackDir)
        {
            var newHealth = Mathf.Max(player.Stats.CurrentHealth - damage, 0);
            player.Health.TakeDamage(newHealth);
            player.Movement.SetKnockback(knockbackDir, player.Stats.KnockbackForce);

            var isDead = newHealth == 0;

            _eventBus.Publish(new PlayerDamagedEvent(player.Id, damage, isDead));

            if (isDead)
                HandleDeath(player, currentMoveStateType);
        }

        private void HandleDeath(PlayerEntity player, MovementStateType currentMoveStateType)
        {
            if (PlayerStateRules.CanCombine(currentMoveStateType, ActionStateType.Death))
            {
                player.Health.StartDead();
                _eventBus.Publish(new PlayerDiedEvent(player.Id));
            }
            else
                BlockAction(player, ActionStateType.Hurt, currentMoveStateType);
        }

        private void BlockAction(PlayerEntity player, ActionStateType actionType, MovementStateType moveState)
        {
            _eventBus.Publish(new PlayerActionBlockedEvent(player.Id, actionType, moveState));
            Debug.Log($"La acción {actionType} está bloqueada con el comando de movimiento: {moveState}");
        }
    }
}
