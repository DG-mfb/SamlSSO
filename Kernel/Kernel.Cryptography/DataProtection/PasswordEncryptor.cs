using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Cryptography.DataProtection
{
    public class PasswordEncryptor : IDisposable
    {
        private int _iterations;
        private SymmetricAlgorithm _algoritm;
        private int _saltSize;
        
        public PasswordEncryptor() :this(new AesCryptoServiceProvider(), 1000, 8)
        { }

        public PasswordEncryptor(SymmetricAlgorithm aesCryptoServiceProvider, int iterations, int saltSize)
        {
            this._algoritm = aesCryptoServiceProvider;
            this._iterations = iterations;
            this._saltSize = saltSize;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (this._algoritm != null)
                this._algoritm.Dispose();
        }

        public byte[] Encrypt(string password, string plainText, out byte[] salt)
        {
            salt = new byte[this._saltSize];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt);
            }

            byte[] encrypted;
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, this._iterations);
            var iv = rfc2898DeriveBytes.GetBytes(this._algoritm.LegalBlockSizes[0].MaxSize / 8);
            var key = rfc2898DeriveBytes.GetBytes(this._algoritm.KeySize / 8);
            
            encrypted = EncryptStringToBytes(plainText, key, iv);
            return encrypted;
        }

        public string Decrypt(string password, byte[] salt, byte[] encrypted)
        {
            string plainText = null;
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, this._iterations);
            var iv = rfc2898DeriveBytes.GetBytes(this._algoritm.LegalBlockSizes[0].MaxSize / 8);
            var key = rfc2898DeriveBytes.GetBytes(this._algoritm.KeySize / 8);

            plainText = DecryptStringFromBytes(encrypted, key, iv);
            return plainText;
        }

        private byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            this._algoritm.Key = key;
            this._algoritm.IV = iv;
            var encryptor = this._algoritm.CreateEncryptor(this._algoritm.Key, this._algoritm.IV);
            
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;
            
            this._algoritm.Key = key;
            this._algoritm.IV = iv;
            
            var decryptor = this._algoritm.CreateDecryptor(this._algoritm.Key, this._algoritm.IV);
            
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }
    }
}