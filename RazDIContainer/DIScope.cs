using System;
using System.Collections.Generic;

namespace RazDIContainer
{
    public class DIScope : IDisposable
    {
        private readonly DIContainer _container;
        private readonly Dictionary<Type, object> _scopedInstances = new Dictionary<Type, object>();

        public DIScope(DIContainer container)
        {
            _container = container;
        }

        public object Resolve(Type type)
        {
            if (_container.TryGetRegistration(type, out var registration))
            {
                if (registration.Lifetime == DIContainer.Lifetime.Scoped)
                {
                    if (!_scopedInstances.ContainsKey(type))
                        _scopedInstances[type] = _container.CreateInstanceWithScope(registration.ConcreteType, this);
                    return _scopedInstances[type];
                }
                return _container.ResolveWithScope(type, this);
            }
            throw new InvalidOperationException($"Type {type.Name} not registered");
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public void Dispose()
        {
            _scopedInstances.Clear();
        }
    }
}