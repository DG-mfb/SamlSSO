using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public class HttpPostResponseOutboundContext : SamlOutboundContext<ISamlForm>
    {
        public HttpPostResponseOutboundContext(ISamlForm form)
        {
            this.Form = form;
        }
        public override Func<ISamlForm, Task> DespatchDelegate { get; set; }
        public ISamlForm Form { get; }
    }
}