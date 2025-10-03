using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(menuName ="Game/AttackData")]
    public class AttackDataSO : ScriptableObject
    {
        public string id;
        public int damage;
        public float windowAfterInput;// para combos
        public string animationTrigger;
    }
}
