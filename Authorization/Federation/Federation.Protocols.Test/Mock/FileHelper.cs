using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Federation.Protocols.Test.Mock
{
    internal class FileHelper
    {
        private const string EncryptedAssertion = "EncryptedAssertion.xml";
        private const string SignedAssertion = "SignedAssertion.xml";
        internal static string GetEncryptedAssertionFilePath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = baseDir.Substring(0, baseDir.IndexOf("bin"));
            path = Path.Combine(path, "Assertions", FileHelper.EncryptedAssertion);
            return path;
        }

        internal static string GetSignedAssertion()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = baseDir.Substring(0, baseDir.IndexOf("bin"));
            path = Path.Combine(path, "Assertions", FileHelper.SignedAssertion);
            return path;
        }
    }
}