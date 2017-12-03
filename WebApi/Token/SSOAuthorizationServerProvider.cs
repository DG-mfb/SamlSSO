using System.Net.Http;
using System.Threading.Tasks;
using Kernel.Authorisation;

namespace WebApi.Token
{
    public class SSOAuthorizationServerProvider : IAuthorizationServerProvider
    {
        async Task IAuthorizationServerProvider.TokenEndpointResponse<TContext>(TContext context)
        {
            var relayState = context.RelayState;
            if (!relayState.ContainsKey("returnUrl"))
                return;
            var returnUrl = relayState["returnUrl"].ToString();
            var httpClient = new HttpClient();
            
            var result = await HttpClientExtensions.PostAsJsonAsync(httpClient, returnUrl, context.Token);
            
            context.RequestCompleted();
        }
    }
}