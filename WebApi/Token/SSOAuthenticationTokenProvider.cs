using Microsoft.Owin.Security.Infrastructure;

namespace WebApi.Token
{
    public class SSOAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        public SSOAuthenticationTokenProvider()
        {
            base.OnCreate = c =>
            {
                var token = c.SerializeTicket();
                c.SetToken(token);
            }; 
        }
    }
}