using UnityEngine.InputSystem;

namespace Game.Input
{
    public interface IReemittableInputStrategy
    {
        void OnReemitInput(InputAction.CallbackContext ctx);
    }
}
