using Game.Core;
using Game.Input;
using Game.Input.Commands;
using Game.Services.Application;
using UnityEngine;

namespace Game.Application
{

    public class PlayerController : MonoBehaviour
    {
        IInputService _inputService;
        IPlayerApplicationService _playerApp;

        private void Awake()
        {
            _inputService ??= Bootstrapper.Container.Resolve<IInputService>();
            _playerApp ??= Bootstrapper.Container.Resolve<IPlayerApplicationService>();
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }

        public void Tick(float dt)
        {
            InputCommand command;
            while (_inputService.TryDequeue(out command))
            {
                _playerApp.ProcessInputCommands(command, dt);
            }

            _playerApp.Update(dt);
        }
    }
}