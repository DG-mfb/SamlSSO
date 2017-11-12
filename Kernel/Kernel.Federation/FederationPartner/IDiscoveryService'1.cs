using System;

namespace Kernel.Federation.FederationPartner
{
    public interface IDiscoveryService<TContext, TResult> : IDiscoveryService<TResult>
    {
        Func<TContext, TResult> Factory { set; }
        TResult ResolveParnerId(TContext context);
    }
}