using System;

namespace Game.Core
{
    public interface IContainer
    {
        void RegisterSingleton<TService>(TService instance);
        void RegisterTransient<TService>(Func<TService> factory);
        TService Resolve<TService>();
        bool TryResolve<TService>(out TService service);
    }
}
