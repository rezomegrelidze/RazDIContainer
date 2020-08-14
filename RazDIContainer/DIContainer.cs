using System;
using System.Collections.Generic;
using System.Linq;

namespace RazDIContainer
{
    public class DIContainer
    {
        private readonly Dictionary<Type, Type> resolver;
        private readonly Dictionary<Type, object> singletons;  

        public DIContainer()
        {
            resolver = new Dictionary<Type, Type>();
            singletons = new Dictionary<Type, object>();
        }

        public void Register<TAbstract, TConcrete>()
        {
            resolver[typeof(TAbstract)] = typeof(TConcrete);
        }

        public void RegisterSingleton<TAbstract, TConcrete>()
        {
            resolver[typeof(TAbstract)] = typeof(TConcrete);
            singletons[typeof(TAbstract)] = Resolve<TAbstract>();
        }

        public TAbstract Resolve<TAbstract>()
        {
            return (TAbstract) Resolve(typeof(TAbstract));
        }

        private object Resolve(Type type)
        {
            if (singletons.ContainsKey(type)) return singletons[type];
            if (HasSingleParameterlessConstructor(type) || !type.GetConstructors().Any())
            {
                return Activator.CreateInstance(resolver[type]) ;
            }

            var constructor = type
                .GetConstructors().First();

            var parameters = constructor.GetParameters().Select(p =>
            {
                if (!resolver.ContainsKey(p.ParameterType)) return Activator.CreateInstance(p.ParameterType);
                var getInstance = Resolve(p.ParameterType);
                return getInstance;
            });

            return Activator.CreateInstance(type, parameters.ToArray());
        }

        private bool HasSingleParameterlessConstructor(Type type)
        {
            var constructors = type.GetConstructors();
            return constructors.Length == 1 && !constructors.Single().GetParameters().Any();
        }
    }
}
