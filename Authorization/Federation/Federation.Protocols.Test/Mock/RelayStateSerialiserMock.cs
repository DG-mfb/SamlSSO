using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Serialisation;

namespace Federation.Protocols.Test.Mock
{
    internal class RelayStateSerialiserMock : IRelayStateSerialiser
    {
        public Task<object> Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> Serialize(object data)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, object[] o)
        {
            throw new NotImplementedException();
        }

        object ISerializer.Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        string ISerializer.Serialize(object o)
        {
            throw new NotImplementedException();
        }
    }
}