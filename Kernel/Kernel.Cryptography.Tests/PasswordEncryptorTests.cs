using System;
using Kernel.Cryptography.DataProtection;
using NUnit.Framework;

namespace Kernel.Cryptography.Tests
{
    [TestFixture]
    public class PasswordEncryptorTests
    {
        [Test]
        public void EncryptDecryptByPassword_same_instance_default_AesCryptoServiceProvider()
        {
            //ARRANGE
            var encryptor = new PasswordEncryptor();
            //var decryptor = new PasswordEncryptor();
            var plainTextSource = "Test data to encrypt";
            var result = String.Empty;
            byte[] salt;
            //ACT
            var encrypted = encryptor.Encrypt("Password1", plainTextSource, out salt);
            result = encryptor.Decrypt("Password1", salt, encrypted);
            //ASSERT
            Assert.AreEqual(plainTextSource, result);
        }

        [Test]
        public void EncryptDecryptByPassword_different_instance_default_AesCryptoServiceProvider()
        {
            //ARRANGE
            var encryptor = new PasswordEncryptor();
            var decryptor = new PasswordEncryptor();
            var plainTextSource = "Test data to encrypt";
            var result = String.Empty;
            byte[] salt;
            //ACT
            var encrypted = encryptor.Encrypt("Password1", plainTextSource, out salt);
            result = decryptor.Decrypt("Password1", salt, encrypted);
            //ASSERT
            Assert.AreEqual(plainTextSource, result);
        }
    }
}