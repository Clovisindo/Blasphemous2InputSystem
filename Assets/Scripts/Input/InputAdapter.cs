using Game.Input.Commands;
using System;
using System.Collections.Generic;

namespace Game.Input
{
    public class InputAdapter : IInputService
    {
        readonly Queue<InputCommand> _queue = new();
        IInputStrategy __strategy;
        PlayerInputActions _actionsAsset;

        public InputAdapter(PlayerInputActions actionsAsset)
        {
            _actionsAsset = actionsAsset ?? throw new ArgumentNullException(nameof(actionsAsset));
        }
       
        // De arranque no queremos inicializar ninguna estrategia
        public void Initialize() { }

        public void SetStrategy(IInputStrategy strategy)
        {
            __strategy?.ShutDown();
            __strategy = strategy;
            if (__strategy != null) 
                __strategy.Initialize(_actionsAsset);
        }

        public void Update(float deltaTime)
        {
            if (__strategy == null) return;
            var cmds = __strategy.Poll(deltaTime);
            if (cmds != null)
            {
                foreach (var c in cmds)
                    _queue.Enqueue(c);
            }
        }

        public bool TryDequeue(out InputCommand command)
        {
            if(_queue.Count > 0) { command = _queue.Dequeue(); return true; }
            command = null;
            return false;
        }

        public void Enqueue(InputCommand command) => _queue.Enqueue(command);

        public void ShutDown()
        {
            __strategy?.ShutDown();
            _queue.Clear();
        }
    }
}
