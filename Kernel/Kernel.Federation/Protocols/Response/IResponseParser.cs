using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Response
{
    public interface IResponseParser<TContext, TResult>
    {
        Task<TResult> ParseResponse(TContext context);
    }
}