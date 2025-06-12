using System;
using System.Collections.Generic;
using System.Linq;

namespace RazDIContainer
{
    /// <summary>
    /// A simple dependency injection container supporting singleton, scoped, and transient lifetimes.
    /// </summary>
    public class DIContainer
    {
        private readonly Dictionary<Type, Registration> registrations = new Dictionary<Type, Registration>();

        /// <summary>
        /// Registers a mapping from an abstract type to a concrete type with the specified lifetime.
        /// </summary>
        /// <typeparam name="TAbstract">The abstract type or interface.</typeparam>
        /// <typeparam name="TConcrete">The concrete implementation type.</typeparam>
        /// <param name="lifetime">The lifetime of the registration (Transient by default).</param>
        public void Register<TAbstract, TConcrete>(Lifetime lifetime = Lifetime.Transient)
        {
            registrations[typeof(TAbstract)] = new Registration
            {
                ConcreteType = typeof(TConcrete),
                Lifetime = lifetime
            };
        }

        /// <summary>
        /// Registers a mapping as a singleton.
        /// </summary>
        /// <typeparam name="TAbstract">The abstract type or interface.</typeparam>
        /// <typeparam name="TConcrete">The concrete implementation type.</typeparam>
        public void RegisterSingleton<TAbstract, TConcrete>()
            => Register<TAbstract, TConcrete>(Lifetime.Singleton);

        /// <summary>
        /// Registers a mapping as scoped.
        /// </summary>
        /// <typeparam name="TAbstract">The abstract type or interface.</typeparam>
        /// <typeparam name="TConcrete">The concrete implementation type.</typeparam>
        public void RegisterScoped<TAbstract, TConcrete>()
            => Register<TAbstract, TConcrete>(Lifetime.Scoped);

        /// <summary>
        /// Registers a mapping as transient.
        /// </summary>
        /// <typeparam name="TAbstract">The abstract type or interface.</typeparam>
        /// <typeparam name="TConcrete">The concrete implementation type.</typeparam>
        public void RegisterTransient<TAbstract, TConcrete>()
            => Register<TAbstract, TConcrete>(Lifetime.Transient);

        /// <summary>
        /// Registers a concrete type as itself.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete implementation type.</typeparam>
        public void Register<TConcrete>()
        {
            Register<TConcrete, TConcrete>();
        }

        /// <summary>
        /// Registers a concrete type as itself with singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete implementation type.</typeparam>
        public void RegisterSingleton<TConcrete>()
        {
            RegisterSingleton<TConcrete, TConcrete>();
        }

        /// <summary>
        /// Registers an existing instance as a singleton for the specified abstract type.
        /// </summary>
        /// <typeparam name="TAbstract">The abstract type or interface.</typeparam>
        /// <param name="instance">The instance to register.</param>
        public void RegisterInstance<TAbstract>(TAbstract instance)
        {
            registrations[typeof(TAbstract)] = new Registration
            {
                ConcreteType = instance.GetType(),
                Lifetime = Lifetime.Singleton,
                SingletonInstance = instance
            };
        }

        /// <summary>
        /// Creates a new scope for resolving scoped dependencies.
        /// </summary>
        /// <returns>A new <see cref="DIScope"/> instance.</returns>
        public DIScope CreateScope()
        {
            return new DIScope(this);
        }

        /// <summary>
        /// Tries to get the registration for a given type.
        /// </summary>
        /// <param name="type">The type to look up.</param>
        /// <param name="registration">The found registration, if any.</param>
        /// <returns>True if found, otherwise false.</returns>
        internal bool TryGetRegistration(Type type, out Registration registration)
        {
            return registrations.TryGetValue(type, out registration);
        }

        /// <summary>
        /// Resolves a type, handling singleton and transient lifetimes. Scoped lifetimes are handled in <see cref="DIScope"/>.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <param name="scope">The current scope, or null for root resolution.</param>
        /// <returns>The resolved instance.</returns>
        internal object ResolveWithScope(Type type, DIScope scope)
        {
            if (registrations.TryGetValue(type, out var registration))
            {
                if (registration.Lifetime == Lifetime.Singleton)
                {
                    if (registration.SingletonInstance == null)
                        registration.SingletonInstance = CreateInstanceWithScope(registration.ConcreteType, t => scope != null ? scope.Resolve(t) : Resolve(t));
                    return registration.SingletonInstance;
                }
                if (registration.Lifetime == Lifetime.Transient)
                    return CreateInstanceWithScope(registration.ConcreteType, t => scope != null ? scope.Resolve(t) : Resolve(t));
                // Scoped handled in DIScope
            }
            throw new InvalidOperationException($"Type {type.Name} not registered");
        }

        /// <summary>
        /// Creates an instance of the specified type, resolving its constructor dependencies using the provided resolver.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="resolver">A function to resolve constructor parameter types.</param>
        /// <returns>The created instance.</returns>
        internal object CreateInstanceWithScope(Type type, Func<Type, object> resolver)
        {
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters()
                .Select(p => resolver(p.ParameterType))
                .ToArray();
            return Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Resolves an instance of the specified abstract type.
        /// </summary>
        /// <typeparam name="TAbstract">The type to resolve.</typeparam>
        /// <returns>The resolved instance.</returns>
        public TAbstract Resolve<TAbstract>()
        {
            return (TAbstract)Resolve(typeof(TAbstract));
        }

        /// <summary>
        /// Resolves an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The resolved instance.</returns>
        public object Resolve(Type type)
        {
            return ResolveWithScope(type, null);
        }
    }
}
