using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface IMessageEncoding
    {
        Task<string> EncodeMessage<TMessage>(TMessage message);
        Task<string> EncodeMessage(byte[] message);
        Task<TMessage> DecodeMessage<TMessage>(string message);
        Task<string> DecodeMessage(string message);
        Task<byte[]> Decode(string message);
    }
}