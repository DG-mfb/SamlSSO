using System.Collections.Generic;

namespace Kernel.Federation.Tokens
{
    public class IssuingAuthority
    {
        public IssuingAuthority(string name)
        {
            this.Name = name;
        }
        public string Name { get; }
        public ISet<string> Issuers { get; }
        public ISet<string> Thumbprints { get; }
    }
}