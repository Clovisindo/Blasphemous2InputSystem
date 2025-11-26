using Game.Input.Commands;
using System.Collections.Generic;
using System.Linq;
using static Utilities;

namespace Game.Input
{
    public enum ComboType { None, LightLightHeavy }
    public class InputBuffer : IInputBuffer
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
            while (_buffer.Count > _maxSize) _buffer.RemoveFirst();
            Prune();
        }
        void Prune()
        {
            var cutoff = UnityEngine.Time.unscaledDeltaTime - _windowTime;
            while (_buffer.First != null && _buffer.First.Value.Timestamp < cutoff)
                _buffer.RemoveFirst();
        }

        /// <summary>
        /// Devuelve el comando más reciente dentro de la ventana sin eliminarlo.
        /// </summary>
        public InputCommand Peek()
        {
            Prune();
            return _buffer.Last?.Value;
        }

        /// <summary>
        /// Devuelve el comando más antiguo dentro de la ventana sin eliminarlo.
        /// </summary>
        public InputCommand PeekFirst()
        {
            Prune();
            return _buffer.First?.Value;
        }

        /// <summary>
        /// Extrae el comando más antiguo y lo elimina.
        /// </summary>
        public bool TryDequeue(out InputCommand command)
        {
            Prune();

            if (_buffer.First == null)
            {
                command = null;
                return false;
            }

            command = _buffer.First.Value;
            _buffer.RemoveFirst();
            return true;
        }

        /// <summary>
        /// Elimina el último comando (por ejemplo, tras una transición exitosa).
        /// </summary>
        public void Consume()
        {
            if (_buffer.Last != null)
                _buffer.RemoveLast();
        }

        /// <summary>
        /// Elimina todos los comandos del buffer.
        /// </summary>
        public void Clear() => _buffer.Clear();

        /// <summary>
        /// Devuelve true si hay algún comando válido dentro de la ventana.
        /// </summary>
        public bool HasRecentCommand()
        {
            Prune();
            return _buffer.Count > 0;
        }

        /// <summary>
        /// Busca si el último comando es de un tipo específico.
        /// </summary>
        public bool LastIs<T>() where T : InputCommand
        {
            Prune();
            return _buffer.Last?.Value is T;
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
