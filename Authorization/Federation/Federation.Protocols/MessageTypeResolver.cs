using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;

namespace Federation.Protocols
{
    internal class MessageTypeResolver : IMessageTypeResolver
    {
        public Type ResolveMessageType(string message, IEnumerable<Type> types)
        {
            using (var reader = XmlReader.Create(new StringReader(message)))
            {
                foreach (var t in types)
                {
                    var fi = t.GetField("ElementName", BindingFlags.Public | BindingFlags.Static);
                    if (fi == null)
                        continue;

                    reader.MoveToContent();
                    if (reader.IsStartElement(fi.GetRawConstantValue().ToString(), Saml20Constants.Protocol))
                        return t;
                }
            }
            throw new NotSupportedException();
        }
    }
}