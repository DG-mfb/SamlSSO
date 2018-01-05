using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Kernel.Federation.Protocols;
using Kernel.Reflection;
using Shared.Federtion.Constants;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request
{
    internal class RequestTypeResolver : IMessageTypeResolver
    {
        public Type ResolveMessageType(string message)
        {
            var requests = GetRequestTypes();
            using (var reader = XmlReader.Create(new StringReader(message)))
            {
                foreach (var t in requests)
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

        private IEnumerable<Type> GetRequestTypes()
        {
            return ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(RequestAbstract).IsAssignableFrom(t));
        }
    }
}