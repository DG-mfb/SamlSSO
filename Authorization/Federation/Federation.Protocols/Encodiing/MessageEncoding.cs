using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kernel.Compression;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Encodiing
{
    internal class MessageEncoding : IMessageEncoding
    {
        private readonly ICompression _compression;
        public MessageEncoding(ICompression compression)
        {
            this._compression = compression;
        }

        [Obsolete("To be removed.", false)]
        public Task<TMessage> DecodeMessage<TMessage>(string message)
        {
            throw new NotImplementedException();
        }

        [Obsolete("To be removed.", false)]
        public async Task<string> DecodeMessage(string message)
        {
            var buffer = Convert.FromBase64String(message);
            var decoded = await Helper.DeflateDecompress(buffer, this._compression);
            using (var reader = new StreamReader(new MemoryStream(decoded), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<byte[]> Decode(string message)
        {
            var buffer = Convert.FromBase64String(message);
            var decoded = await Helper.DeflateDecompress(buffer, this._compression);
            return decoded;
        }

        public async Task<string> EncodeMessage<TMessage>(TMessage message)
        {
            var buffer = Encoding.UTF8.GetBytes(message.ToString());
            return await this.EncodeMessage(buffer);
        }

        public async Task<string> EncodeMessage(byte[] message)
        {
            var encoded = await Helper.DeflateCompress(message, this._compression);
            return Convert.ToBase64String(encoded, Base64FormattingOptions.None);
        }
    }
}