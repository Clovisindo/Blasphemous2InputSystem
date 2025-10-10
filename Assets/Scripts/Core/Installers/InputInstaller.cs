using Game.Input;

namespace Game.Core.Installers
{
    public static class InputInstaller
    {
        public static void Install(IContainer container, PlayerInputActions actions)
        {
            var keyboard = new KeyboardInputStrategy();
            var gamepad = new GamepadInputStrategy();

            var adapter = new InputAdapter(keyboard, gamepad);
            adapter.Initialize(actions);

            container.RegisterSingleton<InputAdapter>(adapter);
            container.RegisterSingleton<IInputService>(adapter);
        }
    }
}
