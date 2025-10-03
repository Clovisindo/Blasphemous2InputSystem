using System;
using System.Collections.Generic;

namespace Game.Core
{
    public class SimpleContainer : IContainer
    {
        readonly Dictionary<Type, object> _singletons = new();
        readonly Dictionary<Type, Func<object>> _transients = new();

        public void RegisterSingleton<TService>(TService instance) => _singletons[typeof(TService)] = instance;

        public void RegisterTransient<TService>(Func<TService> factory) => _transients[typeof(TService)] = () => factory();

        public TService Resolve<TService>()
        {
            if (TryResolve(out TService s)) return s;
            throw new InvalidOperationException($"Service {typeof(TService).Name} not registered.");
        }

        public bool TryResolve<TService>(out TService service)
        {
            if (_singletons.TryGetValue(typeof(TService),out var inst))
            {
                service = (TService)inst;
                return true;
            }
            if(_transients.TryGetValue(typeof(TService),out var fac))
            {
                service = (TService)fac();
                return true;
            }
            service = default;
            return false;
        }
    }
}
