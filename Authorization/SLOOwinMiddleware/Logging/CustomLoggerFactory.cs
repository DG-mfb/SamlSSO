using Kernel.DependancyResolver;
using Microsoft.Owin.Logging;

namespace SLOOwinMiddleware.Logging
{
    internal class CustomLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerFactory _owinLoggerFactory;
        private readonly IDependencyResolver _dependencyResolver;

        public CustomLoggerFactory(IDependencyResolver dependencyResolver, ILoggerFactory owinLoggerFactory)
        {
            this._dependencyResolver = dependencyResolver;
            this._owinLoggerFactory = owinLoggerFactory;
        }
        public ILogger Create(string name)
        {
            if (name == typeof(SLOOwinMiddleware).FullName)
                return this._dependencyResolver.Resolve<CustomLogger>();

            return this._owinLoggerFactory.Create(name);
        }
    }
}