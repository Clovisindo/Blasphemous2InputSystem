using System.Collections.Generic;

namespace Game.Domain.Entities.Player
{
    public enum InvulnerableCapability
    {
        Dash,
        Hurt,
        Invulnerable,
        Shield
    }

    public class DamageController
    {
        private readonly HashSet<InvulnerableCapability> _invulSources = new();

        public bool isInvulnerable => _invulSources.Count > 0;

        public void AddInvulnerability(InvulnerableCapability source) => _invulSources.Add(source);

        public void RemoveInvulnerability(InvulnerableCapability source) => _invulSources.Remove(source);
    }
}
