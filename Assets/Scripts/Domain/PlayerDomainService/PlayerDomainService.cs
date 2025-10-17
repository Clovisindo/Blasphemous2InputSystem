using Game.Domain.Entities;

namespace Game.Domain.Services
{
    /// <summary>
    /// Sirve para operaciones que afectan a varias entidades o involucran lógica compartida (ej. aplicar daño, resolver knockback…).
    /// </summary>
    public class PlayerDomainService
    {
        public void ApplyDamage(PlayerEntity player, int damage)
        {
            player.TakeDamage(damage);
        }
    }
}
