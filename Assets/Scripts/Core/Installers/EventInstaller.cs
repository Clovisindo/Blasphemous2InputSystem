using Game.Events;

namespace Game.Core.Installers
{
    public static class EventInstaller
    {
        public static void Install(IContainer container)
        {
            var eventBus = new EventBus();
            container.RegisterSingleton<IEventBus>(eventBus);
        }
    }
}
