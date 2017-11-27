using System;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Kernel.DependancyResolver;
using Kernel.Federation.MetaData;
using Kernel.Security.CertificateManagement;
using Shared.Federtion.Factories;

namespace Shared.Federtion
{
    public class IdentityConfigurationHelper
    {
        private static IdentityConfiguration _identityConfiguration = new IdentityConfiguration();

        public static IssuerNameRegistry IssuerNameRegistry
        {
            get
            {
                return IdentityConfigurationHelper._identityConfiguration.IssuerNameRegistry;
            }
        }
        /// <summary>
        /// Invoke after metadata is retrieved and parsed
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="dependencyResolver"></param>
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

            var certManager = dependencyResolver.Resolve<ICertificateManager>();
           
            //Default WIF implementation change if another policy is in place. Used to validate the issuer when building the claims
            var identityRegister = IdentityConfigurationHelper._identityConfiguration.IssuerNameRegistry as ConfigurationBasedIssuerNameRegistry;//SecurityTokenHandlerConfiguration.DefaultIssuerNameRegistry as ConfigurationBasedIssuerNameRegistry;
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
                    identityRegister.AddTrustedIssuer(certManager.GetCertificateThumbprint(next), entityId);
                return t;
            });
        }
    }
}