﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Federation.MetaData.Configuration.EntityDescriptors;

namespace Kernel.Federation.FederationPartner
{
    public class AuthnRequestConfiguration : RequestConfiguration
    {
        private readonly FederationPartyAuthnRequestConfiguration _federationPartyAuthnRequestConfiguration;
        public AuthnRequestConfiguration(string requestId, EntityDesriptorConfiguration entityDesriptorConfiguration, FederationPartyAuthnRequestConfiguration federationPartyAuthnRequestConfiguration)
            :base(requestId, federationPartyAuthnRequestConfiguration.Version, entityDesriptorConfiguration)
        {
            if (entityDesriptorConfiguration == null)
                throw new ArgumentNullException("entityDesriptorConfiguration");
           
            if (federationPartyAuthnRequestConfiguration == null)
                throw new ArgumentNullException("federationPartyAuthnRequestConfiguration");
            this._federationPartyAuthnRequestConfiguration = federationPartyAuthnRequestConfiguration;
            this.AudienceRestriction = new List<string> { entityDesriptorConfiguration.EntityId };
            this.ForceAuthn = federationPartyAuthnRequestConfiguration.ForceAuthn;
            this.IsPassive = federationPartyAuthnRequestConfiguration.IsPassive;
            this.EncryptNameId = federationPartyAuthnRequestConfiguration.DefaultNameId.EncryptNameId;
            this.AllowCreateNameIdPolicy = federationPartyAuthnRequestConfiguration.DefaultNameId.AllowCreate;
            this.SupportedNameIdentifierFormats = new List<Uri>();
            this.DefaultNameIdFormat = federationPartyAuthnRequestConfiguration.DefaultNameId.NameIdFormat;
            this.RequestedAuthnContextConfiguration = federationPartyAuthnRequestConfiguration.RequestedAuthnContextConfiguration;
            this.ScopingConfiguration = federationPartyAuthnRequestConfiguration.ScopingConfiguration;
        }

        public bool IsPassive { get; set; }
        public bool ForceAuthn { get; set; }
        public ICollection<string> AudienceRestriction { get; }
        public Uri DefaultNameIdFormat { get; }
        public bool EncryptNameId { get; }
        public bool AllowCreateNameIdPolicy { get; }
        public ushort AssertionConsumerServiceIndex
        {
            get
            {
                var defaultAssertionIndexEndpoint = (ushort)base.SPSSODescriptors.SelectMany(x => x.AssertionConsumerServices)
               .Single(x => x.IsDefault.GetValueOrDefault()).Index;
                return defaultAssertionIndexEndpoint != this._federationPartyAuthnRequestConfiguration.AssertionIndexEndpoint ? this._federationPartyAuthnRequestConfiguration.AssertionIndexEndpoint : defaultAssertionIndexEndpoint;
            }
        }

        public ICollection<Uri> SupportedNameIdentifierFormats { get; }
        public RequestedAuthnContextConfiguration RequestedAuthnContextConfiguration { get; }
        public ScopingConfiguration ScopingConfiguration { get; }
        public FederationPartyAuthnRequestConfiguration FederationPartyAuthnRequestConfiguration
        {
            get
            {
                return this._federationPartyAuthnRequestConfiguration;
            }
        }
    }
}