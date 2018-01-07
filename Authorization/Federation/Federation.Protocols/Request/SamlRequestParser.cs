using System;
using System.Xml;
using Kernel.Federation.Constants;
using Kernel.Logging;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request
{
    internal abstract class SamlRequestParser
    {
        protected readonly ILogProvider LogProvider;

        public SamlRequestParser(ILogProvider logProvider)
        {
            this.LogProvider = logProvider;
        }
        public RequestAbstract Parse(string response)
        {
            return this.ParseInernal(response);
        }
        protected abstract RequestAbstract ParseInernal(string data);
        protected void ReadResponseStatus(XmlReader reader, RequestAbstract request)
        {
            reader.MoveToContent();
            
            var version = reader.GetAttribute("Version");
            request.Version = version;

            var destination = reader.GetAttribute("Destination");
            request.Destination = destination;

            var id = reader.GetAttribute("ID");
            request.Id = id;

            var issueInstant = reader.GetAttribute("IssueInstant");
            //request.IssueInstant = issueInstant;

            var consent = reader.GetAttribute("Consent");
            request.Consent = consent;

            while (!reader.IsStartElement("Issuer", Saml20Constants.Assertion))
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't find status code element.");
            }
            var format = reader.GetAttribute("Format") ?? NameIdentifierFormats.Entity;

            reader.Read();
            var issuer = reader.Value;
            
            var nameId = new NameId { Format = format, Value = issuer };
            request.Issuer = nameId;
            while (!reader.IsStartElement("Status", Saml20Constants.Protocol))
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't find status code element.");
            }
        }
    }
}