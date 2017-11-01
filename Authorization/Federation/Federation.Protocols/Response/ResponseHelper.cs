using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Federation.Protocols.RelayState;
using Kernel.Federation.FederationPartner;
using Kernel.Logging;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseHelper
    {
        internal static ResponseStatus ParseResponseStatus(string response, ILogProvider logProvider)
        {
            using (var reader = new StringReader(response))
            {
                using (var xmlReader = XmlReader.Create(reader))
                {
                    var responseStatus = ResponseHelper.ReadResponseStatus(xmlReader);
                    logProvider.LogMessage(responseStatus.AggregatedMessage);
                    return responseStatus;
                }
            }
        }
        
        internal static ResponseStatus ReadResponseStatus(XmlReader reader)
        {
            var responseStatus = new ResponseStatus();
            
            while (!reader.IsStartElement("Status", Saml20Constants.Protocol))
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't find status code element.");
            }
            
            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Status"))
            {
                if (reader.IsStartElement("StatusCode", Saml20Constants.Protocol))
                {
                    var statusCode = reader.GetAttribute("Value");
                    if (!String.IsNullOrWhiteSpace(statusCode))
                        responseStatus.StatusCodes.Add(statusCode);
                    continue;
                }
                
                if (reader.IsStartElement("StatusMessage", Saml20Constants.Protocol))
                {
                    reader.Read();
                    responseStatus.StatusMessage = reader.Value;
                    continue;
                }

                if (reader.IsStartElement("StatusDetail", Saml20Constants.Protocol))
                {
                    reader.Read();
                    responseStatus.MessageDetails = reader.Value;
                    continue;
                }
            }

            return responseStatus;
        }

        internal static void EnsureSuccessAndThrow(ResponseStatus status)
        {
            if (status.IsSuccess)
                return;
            var msg = status.AggregatedMessage;
            throw new Exception(msg);
        }

        internal static void EnsureRequestPathMatch(object state, Uri path, IFederationPartyContextBuilder federationPartyContextBuilder)
        {
            var relayState = state as IDictionary<string, object>;
            var partnerId = relayState[RelayStateContstants.FederationPartyId].ToString();
            var federationPartner = federationPartyContextBuilder.BuildContext(partnerId);
            var sPSSODescriptors = federationPartner.MetadataContext.EntityDesriptorConfiguration.SPSSODescriptors;
            var assertionServices = sPSSODescriptors.SelectMany(x => x.AssertionConsumerServices, (p, r) => new { r.Location });
            if (!assertionServices.Any(x => x.Location == path))
                throw new Exception(String.Format("Requested path{0} is not in federation party: {1} assertion configuration.", path.AbsoluteUri, partnerId));
        }
    }
}