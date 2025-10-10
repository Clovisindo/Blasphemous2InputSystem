using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Settings;

namespace Game.Core.Installers
{
    public static class GameplayInstaller
    {
        public static void Install(IContainer container)
        {
            container.RegisterSingleton<InputBuffer>(new InputBuffer(maxSize: 12, windowTime: 0.6f));

            container.RegisterTransient(() =>
            new PlayerStateMachine(container.Resolve<PlayerSettingsSO>(),
            container.Resolve<IEventBus>()
            ));
        }
    }
}
