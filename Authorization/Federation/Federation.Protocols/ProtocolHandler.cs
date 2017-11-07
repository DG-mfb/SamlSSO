﻿using System.Threading.Tasks;
using Kernel.Federation.Protocols;

namespace Federation.Protocols
{
    internal class ProtocolHandler<TBinding> : IProtocolHandler<TBinding> where TBinding : IBindingHandler
    {
        private readonly TBinding _bindingHandler;
        

        public ProtocolHandler(TBinding binding)
        {
            this._bindingHandler = binding;
        }
        
        public async Task HandleOutbound(SamlProtocolContext context)
        {
            await this._bindingHandler.HandleOutbound(context.RequestContext);
        }

        public async Task HandleInbound(SamlProtocolContext context)
        {
            await this._bindingHandler.HandleInbound(context.ResponseContext);
        }
    }
}