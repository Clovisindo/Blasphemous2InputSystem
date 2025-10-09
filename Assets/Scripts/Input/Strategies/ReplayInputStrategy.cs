using Game.Input.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    public class ReplayInputStrategy : IInputStrategy
    {
        readonly List<InputCommand> _recordedCommands;
        int _iterator = 0;
        float _startTime;

        // constructor recibe lista grabada con timestamps relativos
        public ReplayInputStrategy(List<InputCommand> recorded)
        {
            _recordedCommands = recorded;
        }

        public InputDeviceType DeviceType => throw new NotImplementedException();

        public void Initialize(PlayerInputActions actionsAsset)
        {
            _startTime = Time.unscaledTime;
        }

        public List<InputCommand> Poll(float deltaTime)
        {
            var now = Time.unscaledTime - _startTime;
            var outList = new List<InputCommand>();
            while (_iterator < _recordedCommands.Count && _recordedCommands[_iterator].Timestamp <= now)
            {
                outList.Add(_recordedCommands[_iterator]);
                _iterator++;
            }
            return outList;
        }

        public void ShutDown() { }
    }
}
