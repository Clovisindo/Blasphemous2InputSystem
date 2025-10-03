using Game.Input.Commands;
using System.Collections.Generic;
using System.Linq;
using static Utilities;

namespace Game.Input
{
    public enum ComboType { None, LightLightHeavy }
    public class InputBuffer
    {
        readonly int _maxSize;
        readonly float _windowTime;
        readonly LinkedList<InputCommand> _buffer = new();

        public InputBuffer(int maxSize = 12, float windowTime = 0.5f)
        {
            _maxSize = maxSize;
            _windowTime = windowTime;
        }

        public void AddCommand(InputCommand command)
        {
            _buffer.AddLast(command);
            while(_buffer.Count > _maxSize) _buffer.RemoveFirst();
            Prune();
        }
        void Prune()
        {
            var cutoff = UnityEngine.Time.unscaledTime - _windowTime;
            while(_buffer.First != null && _buffer.First.Value.Timestamp < cutoff)
                _buffer.RemoveFirst();
        }

        // Ejemplo simple: detecta secuencia Light -> Light -> Heavy en ventana
        public ComboType DetectCombo()
        {
            Prune();
            var list = _buffer.ToList();
            // busca patrón más simple (esto es personalizable con SO)
            for (int i = 0; i <= list.Count - 3; i++)
            {
                if (list[i] is AttackCommand a1 && a1.Type == AttackType.Light &&
                    list[i + 1] is AttackCommand a2 && a2.Type == AttackType.Light &&
                    list[i + 2] is AttackCommand a3 && a3.Type == AttackType.Heavy)
                {
                    _buffer.Clear();
                    return ComboType.LightLightHeavy;
                }
            }
            return ComboType.None;
        }
    }
}
