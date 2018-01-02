using System.Threading.Tasks;
using Kernel.Serialisation;

namespace Kernel.Federation.Protocols
{
    public interface IRequestSerialiser : ISerializer
    {
        Task<string> SerializeAndCompress(object o);
        Task<T> DecompressAndDeserialize<T>(string data);
    }
}