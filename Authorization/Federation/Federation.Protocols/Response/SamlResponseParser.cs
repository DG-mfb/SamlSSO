using System;
using System.Collections.Generic;
using System.Xml;
using Kernel.Federation.Constants;
using Kernel.Logging;
using Shared.Federtion.Models;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal abstract class SamlResponseParser
    {
        protected readonly ILogProvider LogProvider;

        public SamlResponseParser(ILogProvider logProvider)
        {
            this.LogProvider = logProvider;
        }
        public StatusResponse Parse(string response)
        {
            return this.ParseInernal(response);
        }
        protected abstract StatusResponse ParseInernal(string response);
        protected void ReadResponseStatus(XmlReader reader, StatusResponse response)
        {
            reader.MoveToContent();
            
            var responseTo = reader.GetAttribute("InResponseTo");
            response.InResponseTo = responseTo;

            var version = reader.GetAttribute("Version");
            response.Version = version;

            var destination = reader.GetAttribute("Destination");
            response.Destination = destination;

            var id = reader.GetAttribute("ID");
            response.ID = id;

            var issueInstant = reader.GetAttribute("IssueInstant");
            response.IssueInstantString = issueInstant;

            var consent = reader.GetAttribute("Consent");
            response.Consent = consent;

            while (!reader.IsStartElement("Issuer", Saml20Constants.Assertion))
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't find status code element.");
            }
            var format = reader.GetAttribute("Format") ?? NameIdentifierFormats.Entity;

            reader.Read();
            var issuer = reader.Value;
            
            var nameId = new NameId { Format = format, Value = issuer };
            response.Issuer = nameId;
            while (!reader.IsStartElement("Status", Saml20Constants.Protocol))
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't find status code element.");
            }
            
            var status = new Status();
            
            
            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Status"))
            {
                if (reader.IsStartElement("StatusCode", Saml20Constants.Protocol))
                {
                    var statusCode = reader.GetAttribute("Value");
                    var newCode = new StatusCode();
                    if (status.StatusCode == null)
                        status.StatusCode = newCode;
                    else
                    {
                        var temp = status.StatusCode;
                        while (temp.SubStatusCode != null)
                            temp = temp.SubStatusCode;
                        temp.SubStatusCode = newCode;
                    }
                    if (!String.IsNullOrWhiteSpace(statusCode))
                    {
                        newCode.Value = statusCode;
                    }
                    
                    continue;
                }

                if (reader.IsStartElement("StatusMessage", Saml20Constants.Protocol))
                {
                    reader.Read();
                    status.StatusMessage = reader.Value;
                    continue;
                }

                if (reader.IsStartElement("StatusDetail", Saml20Constants.Protocol))
                {
                    status.StatusDetail = new StatusDetail();
                    var elements = new List<XmlElement>();
                    while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "StatusDetail"))
                    {
                        reader.ReadStartElement();
                        var doc = new XmlDocument();
                        var el = doc.ReadNode(reader);
                        if(el is XmlElement)
                            elements.Add((XmlElement)el);
                    }
                    status.StatusDetail.Any = elements.ToArray();
                    continue;
                }
            }
            response.Status = status;
        }
    }
}