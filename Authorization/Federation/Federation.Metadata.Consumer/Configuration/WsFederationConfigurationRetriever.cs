﻿using System;
using System.IdentityModel.Metadata;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.MetaData;
using Kernel.Web;

namespace Federation.Metadata.FederationPartner.Configuration
{
    public class WsFederationConfigurationRetriever : IConfigurationRetriever<MetadataBase>
    {
        private readonly Func<string, IDocumentRetriever> _retriever;
        private readonly IMetadataSerialiser<MetadataBase> _metadataSerialiser;

        private readonly XmlReaderSettings _safeSettings = new XmlReaderSettings()
        {
            DtdProcessing = DtdProcessing.Prohibit
        };

        /// <summary>
        /// Retrieve metadata from a given source and deserialise it
        /// </summary>
        /// <param name="retriever"></param>
        /// <param name="metadataSerialiser"></param>
        public WsFederationConfigurationRetriever(Func<string, IDocumentRetriever> retriever, IMetadataSerialiser<MetadataBase> metadataSerialiser)
        {
            this._metadataSerialiser = metadataSerialiser;
            this._retriever = retriever;
        }

        /// <summary>
        /// Invoke when the metadata has been retrieved
        /// </summary>
        public Action<MetadataBase> MetadataReceivedCallback { get; set; }

        /// <summary>
        /// Retrieve metadata async for pederation party with cancellation token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task<MetadataBase> GetAsync(FederationPartyConfiguration context, CancellationToken cancel)
        {
            return this.GetAsync(context, this._retriever(context.MetadataAddress), cancel);
        }

        private async Task<MetadataBase> GetAsync(FederationPartyConfiguration context, IDocumentRetriever retriever, CancellationToken cancel)
        {
            this._metadataSerialiser.Validator.SetFederationPartyId(context.FederationPartyId);
            if (string.IsNullOrWhiteSpace(context.MetadataAddress))
                throw new ArgumentNullException("address");
            if (retriever == null)
                throw new ArgumentNullException("retriever");
            var str = await retriever.GetDocumentAsync(context.MetadataAddress, cancel);
            var document = str;
            str = null;
            
            using (XmlReader reader = XmlReader.Create(new StringReader(document), this._safeSettings))
            {
                var federationConfiguration =this._metadataSerialiser.Deserialise(reader);
                if(this.MetadataReceivedCallback != null)
                    this.MetadataReceivedCallback(federationConfiguration);
                return federationConfiguration;
            }
        }
    }
}