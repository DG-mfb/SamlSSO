using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Factories
{
    internal class InboundHandleFactory
    {
        private static ConcurrentDictionary<Type, Func<object, SamlInboundContext, Task>> cache = new ConcurrentDictionary<Type, Func<object, SamlInboundContext, Task>>();

        public static Func<object, SamlInboundContext, Task> GetHandleDelegate(Type contextType)
        {
            return InboundHandleFactory.cache.GetOrAdd(contextType, t => InboundHandleFactory.BuildHandleDelegate(t));
        }

       private static Func<object, SamlInboundContext, Task> BuildHandleDelegate(Type contextType)
        {
            var par1 = Expression.Parameter(typeof(object));
            var par2 = Expression.Parameter(typeof(SamlInboundContext));
            var genericType = typeof(IInboundHandler<>).MakeGenericType(contextType);
            var minfo = genericType.GetMethod("Handle");
            var par1Convert = Expression.Convert(par1, genericType);
            var par2Convert = Expression.Convert(par2, contextType);
            var call = Expression.Call(par1Convert, minfo, par2Convert);
            var lambda = Expression.Lambda<Func<object, SamlInboundContext, Task>>(call, par1, par2);
            return lambda.Compile();
        }
    }
}
