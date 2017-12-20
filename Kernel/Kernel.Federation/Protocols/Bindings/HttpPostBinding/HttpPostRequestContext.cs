using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public class HttpPostRequestContext : SamlOutboundContext<ISamlForm>
    {
        public HttpPostRequestContext(ISamlForm form)
        {
            this.Form = form;
        }
        public override Func<ISamlForm, Task> DespatchDelegate { get; set; }
        public ISamlForm Form { get; }
    }
}