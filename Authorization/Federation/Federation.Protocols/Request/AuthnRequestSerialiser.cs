using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Serialisation;
using Serialisation.Xml;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Request
{
    internal class AuthnRequestSerialiser : IAuthnRequestSerialiser
    {
        private readonly IXmlSerialiser _serialiser;
        private readonly IMessageEncoding _messageEncoding;
        private readonly ILogProvider _logProvider;

        public AuthnRequestSerialiser(IXmlSerialiser serialiser, IMessageEncoding messageEncoding, ILogProvider logProvider)
        {
            this._serialiser = serialiser;
            this._messageEncoding = messageEncoding;
            this._logProvider = logProvider;
        }
        
        public async Task<string> SerializeAndCompress(object o)
        {
            var xmlString = ((ISerializer)this).Serialize(o);
            var compressed = await this._messageEncoding.EncodeMessage<string>(xmlString);
            this._logProvider.LogMessage(String.Format("AuthnRequest compressed:\r\n {0}", compressed));
            var encodedEscaped = Uri.EscapeDataString(Helper.UpperCaseUrlEncode(compressed));
            return encodedEscaped;
        }

        async Task<T> IAuthnRequestSerialiser.DecompressAndDeserialize<T>(string data)
        {
            var unescaped = Uri.UnescapeDataString(data);
            var decompressed = await this._messageEncoding.DecodeMessage(unescaped);
            return ((ISerializer)this).Deserialize<T>(decompressed);
        }

        void ISerializer.Serialize(Stream stream, object[] o)
        {
            throw new NotImplementedException();
        }

        string ISerializer.Serialize(object o)
        {
            this._serialiser.XmlNamespaces.Add("samlp", Saml20Constants.Protocol);
            this._serialiser.XmlNamespaces.Add("saml", Saml20Constants.Assertion);

            using (var ms = new MemoryStream())
            {
                this._serialiser.Serialize(ms, new[] { o });
                ms.Position = 0;
                var streamReader = new StreamReader(ms);
                var xmlString = streamReader.ReadToEnd();
                this._logProvider.LogMessage(String.Format("AuthnRequest serialised:\r\n {0}", xmlString));
                
                return xmlString;
            }
        }

        T ISerializer.Deserialize<T>(string data)
        {
            using (var sr = new StringReader(data))
            {
                using (var reader = XmlReader.Create(sr))
                {
                    var result = this._serialiser.Deserialise<T>(reader);
                    return result;
                }
            }
        }

        object ISerializer.Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        object[] ISerializer.Deserialize(Stream stream, IList<Type> messageTypes)
        {
            throw new NotImplementedException();
        }
    }
}