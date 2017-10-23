using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SecurityManagement.CerificateContext
{
    internal class Utility
    {
        internal static string HashSpki(byte[] data)
        {
            var hash = new SHA256CryptoServiceProvider();
            var hashed = hash.ComputeHash(data);
            return Convert.ToBase64String(hashed);
        }

        internal static byte[] ExtractSpkiBlob(X509Certificate2 certificate)
        {
            NativeMethods.CERT_PUBLIC_KEY_INFO subjectPublicKeyInfo = ((NativeMethods.CERT_INFO)Marshal.PtrToStructure(((NativeMethods.CERT_CONTEXT)Marshal.PtrToStructure(certificate.Handle, typeof(NativeMethods.CERT_CONTEXT))).pCertInfo, typeof(NativeMethods.CERT_INFO))).SubjectPublicKeyInfo;
            uint pcbEncoded = 0;
            IntPtr lpszStructType = new IntPtr(8);
            if (!NativeMethods.CryptEncodeObjectEx(1U, lpszStructType, ref subjectPublicKeyInfo, 0U, (IntPtr)null, (byte[])null, ref pcbEncoded))
                throw new Win32Exception(Marshal.GetLastWin32Error());
           
            byte[] pbEncoded = new byte[(int)pcbEncoded];
            if (!NativeMethods.CryptEncodeObjectEx(1U, lpszStructType, ref subjectPublicKeyInfo, 0U, (IntPtr)null, pbEncoded, ref pcbEncoded))
                throw new Win32Exception(Marshal.GetLastWin32Error());
          
            return pbEncoded;
        }

        internal static string GetSubjectKeyIdentifier(X509Certificate2 certificate)
        {
            var extension = certificate.Extensions["2.5.29.14"] as X509SubjectKeyIdentifierExtension;
            if (extension != null)
                return extension.SubjectKeyIdentifier;
            return null;
        }
    }
}