﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Federation.Protocols.Bindings.HttpPost;
using Federation.Protocols.Bindings.HttpRedirect;
using Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders;
using Federation.Protocols.Encodiing;
using Federation.Protocols.RelayState;
using Federation.Protocols.Request;
using Federation.Protocols.Request.ClauseBuilders;
using Federation.Protocols.Request.Handlers;
using Federation.Protocols.Request.Parsers;
using Federation.Protocols.Request.Validation.ValidationRules;
using Federation.Protocols.Response;
using Federation.Protocols.Response.Validation.ValidationRules;
using Federation.Protocols.Tokens;
using Federation.Protocols.Tokens.Validation;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Reflection;
using Shared.Federtion.Factories;
using Shared.Federtion.Models;
using Shared.Initialisation;

namespace Federation.Protocols.Initialisation
{
    public class ProtocolInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 0; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<ResponseHandler>(Lifetime.Transient);
            dependencyResolver.RegisterType<AuthnRequestHandler>(Lifetime.Transient);
            dependencyResolver.RegisterType<SecurityTokenHandler>(Lifetime.Transient);
            dependencyResolver.RegisterType<TokenHandlerConfigurationProvider>(Lifetime.Transient);
            dependencyResolver.RegisterType<ClaimsProvider>(Lifetime.Transient);
            dependencyResolver.RegisterType<MessageEncoding>(Lifetime.Transient);
            dependencyResolver.RegisterType<HttpRedirectBindingHandler>(Lifetime.Transient);
            dependencyResolver.RegisterType<SamlRequestBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<RequestEncoderBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<RelayStateBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<SignatureBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<PostBindingDecoder>(Lifetime.Transient);
            dependencyResolver.RegisterType<RedirectBindingDecoder>(Lifetime.Transient);
            dependencyResolver.RegisterType<Bindings.HttpPost.ClauseBuilders.SamlRequestBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<Bindings.HttpPost.ClauseBuilders.RelayStateBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<Bindings.HttpPost.ClauseBuilders.SignatureBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterType<RequestParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<RelayStateHandler>(Lifetime.Transient);
            dependencyResolver.RegisterType<RelaystateSerialiser>(Lifetime.Transient);
            dependencyResolver.RegisterType<SubjectConfirmationDataValidator>(Lifetime.Transient);
            dependencyResolver.RegisterType<ConditionsValidator>(Lifetime.Transient);
            dependencyResolver.RegisterType<TokenSerialiser>(Lifetime.Transient);
            dependencyResolver.RegisterType<SecurityTokenValidator>(Lifetime.Transient);
            dependencyResolver.RegisterType<RequestSerialiser>(Lifetime.Transient);
            dependencyResolver.RegisterType<PostRequestDispatcher>(Lifetime.Transient);
            dependencyResolver.RegisterType<ResponseDispatcher>(Lifetime.Transient);
            dependencyResolver.RegisterType<AuthnRequestParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<LogoutRequestParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<RedirectRequestDispatcher>(Lifetime.Transient);
            dependencyResolver.RegisterType<ResponseParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<RelayStateAppender>(Lifetime.Transient);
            dependencyResolver.RegisterType<SamlTokenResponseParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<SamlLogoutResponseParser>(Lifetime.Transient);
            RequestHelper.GetAuthnRequestBuilders = () => dependencyResolver.ResolveAll<ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>>();

            dependencyResolver.RegisterFactory<Func<Type, IMetadataHandler>>(() => t => dependencyResolver.Resolve(t) as IMetadataHandler, Lifetime.Singleton);
            this.GetBuilders().Aggregate(dependencyResolver, (r, next) => { r.RegisterType(next, Lifetime.Transient); return r; });

            dependencyResolver.RegisterFactory<Func<Type, SamlResponseParser>>(() => t =>
            {
                var toResolve = typeof(IMessageParser<,>).MakeGenericType(typeof(string), t);
                return (SamlResponseParser)dependencyResolver.Resolve(toResolve);
            }, Lifetime.Singleton);
            dependencyResolver.RegisterFactory<Func<Type, SamlRequestParser>>(() => t =>
            {
                var toResolve = typeof(IMessageParser<,>).MakeGenericType(typeof(string), t);
                return (SamlRequestParser)dependencyResolver.Resolve(toResolve);
            }, Lifetime.Singleton);

            dependencyResolver.RegisterFactory<Func<Type, object>>(() => dependencyResolver.Resolve, Lifetime.Transient);
            dependencyResolver.RegisterFactory< Func<string, IProtocolHandler> >(() =>
            {
                return b =>
                {
                    if (b == Kernel.Federation.MetaData.Configuration.Bindings.Http_Redirect)
                    {
                        return new ProtocolHandler<HttpRedirectBindingHandler>(new HttpRedirectBindingHandler(dependencyResolver));
                    }
                    if (b == Kernel.Federation.MetaData.Configuration.Bindings.Http_Post)
                    {
                        var bh = dependencyResolver.Resolve<HttpPostBindingHandler>();
                        return new ProtocolHandler<HttpPostBindingHandler>(bh);
                    }
                    throw new NotSupportedException();

                };
            }, Lifetime.Singleton);
            dependencyResolver.RegisterFactory<Func<IEnumerable<IRedirectClauseBuilder>>>(() =>
            {
                return () => dependencyResolver.ResolveAll<IRedirectClauseBuilder>();
            }, Lifetime.Singleton);

            dependencyResolver.RegisterFactory<Func<IEnumerable<IPostClauseBuilder>>>(() =>
            {
                return () => dependencyResolver.ResolveAll<IPostClauseBuilder>();
            }, Lifetime.Singleton);

            dependencyResolver.RegisterFactory<Func<IEnumerable<RequestValidationRule>>>(() => () =>
            {
                var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(RequestValidationRule).IsAssignableFrom(t));
                var rules = types.Select(t => dependencyResolver.Resolve(t))
                    .Cast<RequestValidationRule>();
                return rules;
            }, Lifetime.Singleton);

            dependencyResolver.RegisterFactory<Func<IEnumerable<ResponseValidationRule>>>(() => () =>
            {
                var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(ResponseValidationRule).IsAssignableFrom(t));
                var rules = types.Select(t => dependencyResolver.Resolve(t))
                    .Cast<ResponseValidationRule>();
                return rules;
            }, Lifetime.Singleton);

            return Task.CompletedTask;
        }

        private IEnumerable<Type> GetBuilders()
        {
            return ReflectionHelper.GetAllTypes(new[] { typeof(AutnRequestClauseBuilder).Assembly }, t => RequestHelper.Condition(t));
        }
    }
}