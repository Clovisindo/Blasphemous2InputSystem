using Game.Domain.Entities;
using UnityEngine;

namespace Game.Domain.Services
{
    public interface IPlayerDomainService
    {
        void ApplyDamage(PlayerEntity player, Utilities.MovementStateType currentMoveStateType, int damage, Vector2 knockbackDir);
    }
}