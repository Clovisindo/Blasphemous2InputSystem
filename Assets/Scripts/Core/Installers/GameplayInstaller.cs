using Game.Domain.Entities;
using Game.Domain.StateMachine;
using Game.Events;
using Game.Input;
using Game.Services.Application;
using Game.Settings;

namespace Game.Core.Installers
{
    public static class GameplayInstaller
    {
        public static void Install(IContainer container)
        {
            container.RegisterSingleton<PlayerEntity>(new PlayerEntity(
                new PlayerStats(5f, 100, 10),
                container.Resolve<IEventBus>()));

            container.RegisterTransient(() =>
            new PlayerStateMachine(container.Resolve<PlayerEntity>() ,
            container.Resolve<IEventBus>()
            ));
            
            container.RegisterSingleton<IPlayerApplicationService>(
                new PlayerApplicationService(
                    container.Resolve<PlayerStateMachine>(),
                    container.Resolve<InputBuffer>()
                    ));
        }
    }
}
