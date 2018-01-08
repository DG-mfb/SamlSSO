namespace Kernel.Cryptography
{
    public class DataSignatureDescriptor
    {
        public DataSignatureDescriptor(string signatureAlgorithm, string signature)
        {
            this.SignatureAlgorithm = signatureAlgorithm;
            this.Signature = signature;
        }
        public string SignatureAlgorithm { get; }
        public string Signature { get; }

    }
}
