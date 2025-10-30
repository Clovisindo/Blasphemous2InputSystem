using Game.Domain.Entities;
using Game.Events;
using UnityEngine;
using static Game.Events.PlayerEvents.PlayerEvents;

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

        public void ApplyDamage(PlayerEntity player, int damage)
        {
            if (player.IsInvulnerable) return;

            var newHealth = Mathf.Max(player.Stats.CurrentHealth - damage, 0);
            player.TakeDamage(newHealth);

            var isDead = newHealth == 0;

            //avisa appservice para cambiar la maquina de estados
            _eventBus.Publish(new PlayerDamagedEvent(player.Id,damage,isDead));

            if (isDead)
            {
                player.StartDead();
                _eventBus.Publish(new PlayerDiedEvent(player.Id));
            }
            else
            {
                player.StartHurt();//?? revisar estas cargas
            }

        }
    }
}
