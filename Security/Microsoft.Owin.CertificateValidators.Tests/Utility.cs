using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators.Tests
{
    internal class Utility
    {
        internal static byte[] GetSubjectPublicKeyInfo(X509Certificate2 cert)
        {
            var minfo = typeof(CertificateSubjectPublicKeyInfoValidator).GetMethod("ExtractSpkiBlob", BindingFlags.Static | BindingFlags.NonPublic);
            var expPar = Expression.Parameter(typeof(X509Certificate2));
            var call = Expression.Call(minfo, expPar);
            var lambda = Expression.Lambda<Func<X509Certificate2, byte[]>>(call, expPar)
                .Compile();
            return lambda(cert);
        }

        internal static string HashSpki(byte[] data)
        {
            var hash = new SHA256CryptoServiceProvider();
            var hashed = hash.ComputeHash(data);
            return Convert.ToBase64String(hashed);
        }
    }
}