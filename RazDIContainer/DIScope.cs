using System;
using System.Collections.Generic;

namespace RazDIContainer
{
    /// <summary>
    /// Represents a scope for resolving scoped dependencies in the DI container.
    /// </summary>
    public class DIScope : IDisposable
    {
        private readonly DIContainer _container;
        private readonly Dictionary<Type, object> _scopedInstances = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DIScope"/> class.
        /// </summary>
        /// <param name="container">The DI container that created this scope.</param>
        public DIScope(DIContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves an instance of the specified type within this scope.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The resolved instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the type is not registered.</exception>
        public object Resolve(Type type)
        {
            if (_container.TryGetRegistration(type, out var registration))
            {
                if (registration.Lifetime == Lifetime.Scoped)
                {
                    if (!_scopedInstances.ContainsKey(type))
                        _scopedInstances[type] = _container.CreateInstanceWithScope(registration.ConcreteType, Resolve);
                    return _scopedInstances[type];
                }
                return _container.ResolveWithScope(type, this);
            }
            throw new InvalidOperationException($"Type {type.Name} not registered");
        }

        /// <summary>
        /// Resolves an instance of the specified type within this scope.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The resolved instance.</returns>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Disposes the scope and clears all scoped instances.
        /// </summary>
        public void Dispose()
        {
            _scopedInstances.Clear();
        }
    }
}