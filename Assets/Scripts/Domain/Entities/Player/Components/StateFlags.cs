namespace Game.Domain.Entities.Player
{
    public class StateFlags
    {
        public bool IsGrounded { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsDashing { get; set; }
        public bool IsHurt { get; set; }
        public bool IsDead { get; set; }
        public bool IsInvulnerable { get; set; }
    }
}
