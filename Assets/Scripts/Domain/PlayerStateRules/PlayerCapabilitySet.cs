using System.Collections.Generic;

namespace Game.Domain.Entities
{
    public enum MoveCapability
    {
        Move,
        Jump,
        Dash,
        IsGrounded,
        IsAttacking,
        IsDead
    }

    public class PlayerCapabilitySet
    {
        private readonly HashSet<MoveCapability> _capabilities = new();

        public bool Has(MoveCapability cap) => _capabilities.Contains(cap);
        public void Add(MoveCapability cap) => _capabilities.Add(cap);
        public void Remove(MoveCapability cap) => _capabilities.Remove(cap);
    }
}
