﻿using System.Threading.Tasks;
using DeflateCompression;
using NUnit.Framework;

namespace Federation.Protocols.Test
{
    [TestFixture]
    internal class HelperTests
    {
        [Test]
        public async Task CompressDecompressString()
        {
            //ARRANGE
            var str = "String to compress.";
            var compression = new DeflateCompressor();

            //ACT
            var buffer = System.Text.Encoding.UTF8.GetBytes(str);
            var compressed = await Helper.DeflateCompress(buffer, compression);
            var decompressed = await Helper.DeflateDecompress(compressed, compression);

            //ASSERT
            Assert.AreEqual(str, decompressed);
        }
    }
}