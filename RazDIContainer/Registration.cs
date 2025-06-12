using System;

namespace RazDIContainer
{
    /// <summary>
    /// Represents a service registration in the dependency injection container.
    /// </summary>
    internal class Registration
    {
        /// <summary>
        /// Gets or sets the concrete type to instantiate for this registration.
        /// </summary>
        public Type ConcreteType { get; set; }

        /// <summary>
        /// Gets or sets the lifetime of the registration (Transient, Singleton, or Scoped).
        /// </summary>
        public Lifetime Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the singleton instance for this registration, if applicable.
        /// </summary>
        public object SingletonInstance { get; set; }
    }
}
