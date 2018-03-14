using System.Threading.Tasks;

namespace Kernel.Authentication.Services
{
    public interface IIdentityProviderService
    {
        Task<AuthenticationResult> AuthenticateUser(AuthenticationTypesContext context);
        Task<UserInfoResult> FindUserByEmail(string email);
        Task<string> GeneratePasswordResetToken(string userId);
        Task<bool> ValidateResetPasswordTokenAsync(string userId, string token);
        Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
    }
}