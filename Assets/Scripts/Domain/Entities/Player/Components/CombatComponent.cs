using Game.Events;
using static Game.Events.PlayerEvents.PlayerEvents;
using static Utilities;

namespace Game.Domain.Entities.Player
{
    public class CombatComponent
    {
        readonly PlayerEntity _player;
        readonly IEventBus _eventBus;

        public CombatComponent(PlayerEntity player, IEventBus eventBus)
        {
            _player = player;
            _eventBus = eventBus;
        }

        public void StartAttack(AttackType type)
        {
            _player.Flags.IsAttacking = true;
            _eventBus.Publish(new PlayerAttackStarted(_player.Id, type));
        }

        public void StopAttack()
        {
            _player.Flags.IsAttacking = false;
            _eventBus.Publish(new PlayerAttackFinished(_player.Id));
        }
    }
}
