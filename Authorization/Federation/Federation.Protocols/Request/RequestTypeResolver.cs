using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Federation.Protocols;
using Kernel.Reflection;
using Kernel.Reflection.Extensions;
using Shared.Federtion.Constants;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request
{
    internal class RequestTypeResolver : IMessageTypeResolver
    {
        public Type ResolveMessageType(string message)
        {
            var requests = GetRequestTypes();
            foreach (var t in requests)
            {
                var fi = t.GetField("ElementName", BindingFlags.Public | BindingFlags.Static);
                if (fi == null)
                    continue;
                using (var reader = XmlReader.Create(new StringReader(message)))
                {
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
