using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface IMessageParser<TContext, TResult>
    {
        Task<TResult> Parse(TContext context);
    }
}