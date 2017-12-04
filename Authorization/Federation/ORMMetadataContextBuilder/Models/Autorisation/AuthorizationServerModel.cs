using Kernel.Data;

namespace ORMMetadataContextProvider.Models.Autorisation
{
    public class AuthorizationServerModel : BaseModel
    {
        public string TokenResponseUrl { get; set; }
        public bool UseTokenAuthorisation { get; set; }
        public virtual FederationPartySettings FederationPartySettings { get; protected set; }
    }
}