namespace RazDIContainer
{
    /// <summary>
    /// Specifies the lifetime of a service in the dependency injection container.
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// A new instance is created every time the service is requested.
        /// </summary>
        Transient,

        /// <summary>
        /// A single instance is created and shared for the lifetime of the container.
        /// </summary>
        Singleton,

        /// <summary>
        /// A single instance is created and shared within a scope.
        /// </summary>
        Scoped
    }
}
