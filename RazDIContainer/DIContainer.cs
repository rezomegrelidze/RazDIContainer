using System;
using System.Collections.Generic;
using System.Linq;

namespace RazDIContainer
{
    public class DIContainer
    {

        

        private readonly Dictionary<Type, Registration> registrations = new Dictionary<Type, Registration>();

        public void Register<TAbstract, TConcrete>(Lifetime lifetime = Lifetime.Transient)
        {
            registrations[typeof(TAbstract)] = new Registration
            {
                ConcreteType = typeof(TConcrete),
                Lifetime = lifetime
            };
        }

        public void RegisterSingleton<TAbstract, TConcrete>()
            => Register<TAbstract, TConcrete>(Lifetime.Singleton);

        public void RegisterScoped<TAbstract, TConcrete>()
            => Register<TAbstract, TConcrete>(Lifetime.Scoped);

        public void RegisterTransient<TAbstract, TConcrete>()
            => Register<TAbstract, TConcrete>(Lifetime.Transient);

        public void Register<TConcrete>()
        {
            Register<TConcrete,TConcrete>();
        }

        public void RegisterSingleton<TConcrete>()
        {
            RegisterSingleton<TConcrete,TConcrete>();
        }

        public void RegisterInstance<TAbstract>(TAbstract instance)
        {
            registrations[typeof(TAbstract)] = new Registration
            {
                ConcreteType = instance.GetType(),
                Lifetime = Lifetime.Singleton,
                SingletonInstance = instance
            };
        }

        public DIScope CreateScope()
        {
            return new DIScope(this);
        }

        internal bool TryGetRegistration(Type type, out Registration registration)
        {
            return registrations.TryGetValue(type, out registration);
        }

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

        internal object CreateInstanceWithScope(Type type, Func<Type, object> resolver)
        {
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters()
                .Select(p => resolver(p.ParameterType))
                .ToArray();
            return Activator.CreateInstance(type, parameters);
        }

        public TAbstract Resolve<TAbstract>()
        {
            return (TAbstract)Resolve(typeof(TAbstract));
        }

        public object Resolve(Type type)
        {
            return ResolveWithScope(type, null);
        }

    }
}
