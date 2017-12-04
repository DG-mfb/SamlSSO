using System.Security.Cryptography;
using System.Web.Security;

namespace Kernel.Cryptography.DataProtection
{
    internal class MachineDataProtectorImplementation : DataProtector
    {
        internal MachineDataProtectorImplementation(string applicationName, string primaryPurpose, string[] specificPurposes) 
            : base(applicationName, primaryPurpose, specificPurposes)
        {
        }

        public override bool IsReprotectRequired(byte[] encryptedData)
        {
            return true;
        }

        protected override byte[] ProviderProtect(byte[] userData)
        {
            return MachineKey.Protect(userData, base.PrimaryPurpose);
        }

        protected override byte[] ProviderUnprotect(byte[] encryptedData)
        {
            return MachineKey.Unprotect(encryptedData, base.PrimaryPurpose);
        }
    }
}