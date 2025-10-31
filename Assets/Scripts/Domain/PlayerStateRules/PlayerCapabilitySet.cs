using System.Collections.Generic;

namespace Game.Domain.Entities
{
    public enum Capability
    {
        Move,
        Jump,
        Attack,
        Dash
    }

    public class PlayerCapabilitySet
    {
        private readonly HashSet<Capability> _disabled = new();

        public bool Has(Capability cap) => !_disabled.Contains(cap);
        public void Disable(Capability cap) => _disabled.Add(cap);
        public void Enable(Capability cap) => _disabled.Remove(cap);
    }
}
