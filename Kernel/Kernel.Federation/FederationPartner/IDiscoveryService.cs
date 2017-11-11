using System;

namespace Kernel.Federation.FederationPartner
{
    public interface IDiscoveryService<TContext, TResult>
    {
        Func<TContext, TResult> Factory { set; }
        TResult ResolveParnerId(TContext context);
    }
}