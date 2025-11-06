using Game.Events;
using System;
using static Game.Events.PlayerEvents.PlayerEvents;

namespace Game.Domain.Entities.Player
{
    public class HealthComponent
    {
        readonly PlayerEntity _player;
        readonly IEventBus _eventBus;

        public HealthComponent(PlayerEntity player, IEventBus eventBus)
        {
            _player = player;
            _eventBus = eventBus;
        }

        public void TakeDamage(int newHealth)
        {
            _player.UpdateHealth(_player.Stats.WithHealth(newHealth));
            //evento de cambio vida UI
        }

        public void StartHurt(Guid playerId)
        {
            _eventBus.Publish(new PlayerHurtAnimStart(playerId));
            _player.Flags.IsHurt = true;
            _player.Flags.IsInvulnerable = true;
        }
        public void StopHurt(Guid playerId)
        {
            _eventBus.Publish(new PlayerHurtAnimEnd(playerId));
            _player.Flags.IsHurt = false;
            _player.Flags.IsInvulnerable = false;
        }
        public void StartDead() => _player.Flags.IsDead = true;
        public void StopDead() => _player.Flags.IsDead = false;
    }
}
