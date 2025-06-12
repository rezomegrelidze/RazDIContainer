using System;

namespace RazDIContainer
{
    public enum Lifetime
        {
            Transient,
            Singleton,
            Scoped
        }

    internal class Registration
        {
            public Type ConcreteType { get; set; }
            public Lifetime Lifetime { get; set; }
            public object SingletonInstance { get; set; }
        }
}
