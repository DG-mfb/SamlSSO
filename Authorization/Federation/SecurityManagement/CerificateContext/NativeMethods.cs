using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SecurityManagement.CerificateContext
{
    [Localizable(false)]
    internal static class NativeMethods
    {
        public const int X509_ASN_ENCODING = 1;
        public const int X509_PUBLIC_KEY_INFO = 8;

        [DllImport("crypt32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptEncodeObject(uint dwCertEncodingType, IntPtr lpszStructType, ref NativeMethods.CERT_PUBLIC_KEY_INFO pvStructInfo, byte[] pbEncoded, ref uint pcbEncoded);
        [DllImport("crypt32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptEncodeObjectEx(uint dwCertEncodingType, IntPtr lpszStructType, ref NativeMethods.CERT_PUBLIC_KEY_INFO pvStructInfo, uint dwFlags, IntPtr pEncodePara, byte[] pbEncoded, ref uint pcbEncoded);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CRYPT_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        internal struct CERT_CONTEXT
        {
            public int dwCertEncodingType;
            public IntPtr pbCertEncoded;
            public int cbCertEncoded;
            public IntPtr pCertInfo;
            public IntPtr hCertStore;
        }

        internal struct CRYPT_ALGORITHM_IDENTIFIER
        {
            public string pszObjId;
            public NativeMethods.CRYPT_BLOB Parameters;
        }

        internal struct CRYPT_BIT_BLOB
        {
            public int cbData;
            public IntPtr pbData;
            public int cUnusedBits;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CERT_PUBLIC_KEY_INFO
        {
            public NativeMethods.CRYPT_ALGORITHM_IDENTIFIER Algorithm;
            public NativeMethods.CRYPT_BIT_BLOB PublicKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class CERT_INFO
        {
            public int dwVersion;
            public NativeMethods.CRYPT_BLOB SerialNumber;
            public NativeMethods.CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
            public NativeMethods.CRYPT_BLOB Issuer;
            public System.Runtime.InteropServices.ComTypes.FILETIME NotBefore;
            public System.Runtime.InteropServices.ComTypes.FILETIME NotAfter;
            public NativeMethods.CRYPT_BLOB Subject;
            public NativeMethods.CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;
            public NativeMethods.CRYPT_BIT_BLOB IssuerUniqueId;
            public NativeMethods.CRYPT_BIT_BLOB SubjectUniqueId;
            public int cExtension;
            public IntPtr rgExtension;
        }
    }
}