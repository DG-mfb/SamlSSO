using Kernel.Cryptography.DataProtection;

namespace WebApi.Token
{
    public class MachineKeyDataProtector : Microsoft.Owin.Security.DataProtection.IDataProtector
    {
        private readonly MachineDataProtectorImplementation _protector;
        public MachineKeyDataProtector()
        {
            this._protector = new MachineDataProtectorImplementation("Auth service", "Microsoft.Owin.Security.IDataProtector", new[] { "Saml2" });
        }
        public byte[] Protect(byte[] userData)
        {
            return this._protector.Protect(userData);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return this._protector.Unprotect(protectedData);
        }
    }
}