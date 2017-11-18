using System.Threading.Tasks;
using Kernel.Serialisation;

namespace Kernel.Federation.Protocols
{
    public interface IAuthnRequestSerialiser : ISerializer
    {
        Task<string> SerializeAndCompress(object o);
        Task<T> DecompressAndDeserialize<T>(string data);
    }
}