using Game.Input.Commands;
using System.Collections.Generic;

namespace Game.Input
{ 
    public interface IInputStrategy
    {
        InputDeviceType DeviceType { get; }
        // Arranca la estrategia (binding al InputActionAsset, listeners, etc.)
        void Initialize(PlayerInputActions actionsAsset);

        // Poll es llamado desde el InputAdapter cada frame
        // Debe devolver 0 a N InputCommand generados en este tick.
        List<InputCommand> Poll(float deltaTime);
        void ShutDown();
    }
}
