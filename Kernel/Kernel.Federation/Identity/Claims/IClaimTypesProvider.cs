namespace Kernel.Federation.Identity.Claims
{
    interface IClaimTypesProvider
    {
        FederatiionPartyClaimTypes GetClaims(string parnerId);
    }
}