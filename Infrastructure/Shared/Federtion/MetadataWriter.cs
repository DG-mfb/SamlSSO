﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Federation.MetaData;

namespace Shared.Federtion
{
    /// <summary>
    /// Writes metadata xml to a stream
    /// </summary>
    public abstract class MetadataWriter : IFederationMetadataWriter
    {
        public Task Write(XmlElement xml, MetadataPublicationContext target)
        {
            if (xml == null)
                throw new ArgumentNullException("xmlElement");

            if (target == null)
                throw new ArgumentNullException("target");

            if (!this.CanWrite(target))
                return Task.CompletedTask;

            var writer = new StreamWriter(target.TargetStream);
            using (var w = XmlWriter.Create(writer, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                xml.WriteTo(w);
            }
            return Task.CompletedTask;
        }

        protected abstract bool CanWrite(MetadataPublicationContext target);
    }
}