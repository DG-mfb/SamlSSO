namespace Kernel.Initialisation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kernel.DependancyResolver;

    /// <summary>
    /// Initialises the server
    /// </summary>
    public interface IServerInitialiser
	{
		/// <summary>
		/// Initialises the specified dependency resolver.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver.</param>
		/// <returns></returns>
		Task Initialise(IDependencyResolver dependencyResolver);
        Task Initialise(IDependencyResolver dependencyResolver, Func<IInitialiser, bool> condition);
        Task InitialiseAsync(IEnumerable<IInitialiser> initialisers, IDependencyResolver dependencyResolver, Func<IInitialiser, bool> condition);
    }
}
