using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu (menuName ="Game/PlayerSettings")]
    public class PlayerSettingsSO : ScriptableObject
    {
        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public AttackDataSO[] attacks;
    }
}
