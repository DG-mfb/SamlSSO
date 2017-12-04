namespace Kernel.Cryptography.DataProtection
{
    public class MachineKeyDataProtector : IDataProtector
    {
        private readonly MachineDataProtectorImplementation _protector;
        public MachineKeyDataProtector(string applicationName, string primaryPurpose, string[] specificPurposes)
        {
            this._protector = new MachineDataProtectorImplementation(applicationName, primaryPurpose, specificPurposes);
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