using Game.Domain.StateMachine;
using Game.Input;
using Game.Input.Commands;
using Game.Services.Application;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Game.Services.Application
{
    public class PlayerApplicationService : IPlayerApplicationService
    {
        readonly PlayerStateMachine _stateMachine;
        readonly InputBuffer _buffer;

        public PlayerApplicationService(PlayerStateMachine stateMachine, InputBuffer buffer)
        {
            _stateMachine = stateMachine;
            _buffer = buffer;
        }

        public void ProcessInputCommands(InputCommand command, float deltaTime)
        {
           // Añadimos al buffer antes de procesar
            _buffer.AddCommand(command);

            // Pasamos el comando al dominio (StateMachine)
            _stateMachine.ProcessCommand(command);
        }

        public void Update(float deltaTime)
        {
            _stateMachine.Update(deltaTime);

            // Aquí puedes analizar combos o detectar inputs secuenciales
            var detectedCombo = _buffer.DetectCombo();
            if (detectedCombo != ComboType.None)
                _stateMachine.ExecuteCombo(detectedCombo);
        }
    }
}
