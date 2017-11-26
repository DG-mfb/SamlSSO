using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Response
{
    public interface IResponseParser<TResponse, TResult>
    {
        Task<TResult> ParseResponse(TResponse context);
    }
}