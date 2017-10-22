using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Kernel.DependancyResolver;
using Kernel.Federation.MetaData;
using Shared.Federtion.Factories;

namespace Federation.Metadata.FederationPartner.Configuration
{
    internal class ConfigurationHelper
    {
        public static void OnReceived(MetadataBase metadata, IDependencyResolver dependencyResolver)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            if (dependencyResolver == null)
                throw new ArgumentNullException("dependencyResolver");
            
            string entityId = "RegisteredIssuer";
            var handlerType = typeof(IMetadataHandler<>).MakeGenericType(metadata.GetType());
            var handler = dependencyResolver.Resolve(handlerType) as IMetadataHandler;
            if (handler == null)
                throw new InvalidOperationException(String.Format("Handler must implement: {0}", typeof(IMetadataHandler).Name));
            
            var idps = handler.GetIdentityProviderSingleSignOnDescriptor(metadata);

            var identityRegister = SecurityTokenHandlerConfiguration.DefaultIssuerNameRegistry as ConfigurationBasedIssuerNameRegistry;
            if (identityRegister == null)
                return;

            var register = idps.SelectMany(x => x.Keys.SelectMany(y => y.KeyInfo.Select(cl =>
            {
                var binaryClause = cl as BinaryKeyIdentifierClause;
                if (binaryClause == null)
                    throw new InvalidOperationException(String.Format("Expected type: {0} but it was: {1}", typeof(BinaryKeyIdentifierClause), cl.GetType()));

                var certContent = binaryClause.GetBuffer();
                var cert = new X509Certificate2(certContent);
                return cert;
            }))).Aggregate(identityRegister, (t, next) => 
            {
                if (!identityRegister.ConfiguredTrustedIssuers.Keys.Contains(next.Thumbprint))
                    identityRegister.AddTrustedIssuer(next.Thumbprint, entityId);
                return t;
            });
        }
    }
}