namespace WebApi.Token
{
    public class MachineKeyDataProtector : Kernel.Cryptography.DataProtection.MachineKeyDataProtector, Microsoft.Owin.Security.DataProtection.IDataProtector
    {
        public MachineKeyDataProtector(string applicationName, string primaryPurpose, string[] specificPurposes) : base(applicationName, primaryPurpose, specificPurposes)
        {
        }
    }
}