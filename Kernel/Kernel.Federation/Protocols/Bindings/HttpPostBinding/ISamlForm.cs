namespace Kernel.Federation.Protocols.Bindings.HttpPostBinding
{
    public interface ISamlForm
    {
        string ActionURL { get; set; }
        void SetRequest(string request);
        void SetRelatState(string state);
    }
}