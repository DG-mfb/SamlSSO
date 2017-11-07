namespace Kernel.Federation.Protocols
{
    public class SamlProtocolContext
    {
        public SamlOutboundContext RequestContext { get; set; }
        public SamlInboundContext ResponseContext { get; set; }
    }
}