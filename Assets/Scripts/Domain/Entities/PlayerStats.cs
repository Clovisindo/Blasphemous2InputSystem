using System;

namespace Game.Domain.Entities
{
    /// <summary>
    /// Cualquier cambio en las stats debe gestionarse internamente
    /// </summary>
    [Serializable]
    public class PlayerStats
    {
        public float Speed { get; private set; }
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public int AttackDamage { get; private set; }
        public float JumpForce { get; private set; }
        public float Gravity { get; private set; }
        public float DashSpeed { get; private set; }

        public PlayerStats(float speed, int maxHealth, int attackDamage, float jumpForce, float gravity, float dashSpeed)
        {
            Speed = speed;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            AttackDamage = attackDamage;
            JumpForce = jumpForce;
            Gravity = gravity;
            DashSpeed = dashSpeed;
        }

        private PlayerStats(float speed, int maxHealth, int currentHealth, int attackDamage, float jumpForce, float gravity, float dashSpeed)
        {
            Speed = speed;
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
            AttackDamage = attackDamage;
            JumpForce = jumpForce;
            Gravity = gravity;
            DashSpeed = dashSpeed;
        }

        public PlayerStats WithHealth(int newHealth)
           => new PlayerStats(Speed, MaxHealth, newHealth, AttackDamage, JumpForce,Gravity, DashSpeed);
    }
}