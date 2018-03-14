using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.EntityFramework.IdentityProvider.Managers;
using AspNet.EntityFramework.IdentityProvider.Models;
using Kernel.Authentication;
using Kernel.Authentication.Claims;
using Kernel.Authentication.Services;

namespace AspNet.EntityFramework.IdentityProvider
{
    internal class IdentityProviderService : IIdentityProviderService
    {
        private readonly ApplicationUserManager _manager;
        private readonly IUserClaimsProvider<ApplicationUser> _claimsProvider;

        public IdentityProviderService(ApplicationUserManager manager, IUserClaimsProvider<ApplicationUser> claimsProvider)
        {
            this._manager = manager;
            this._claimsProvider = claimsProvider;
        }
        public async Task<AuthenticationResult> AuthenticateUser(AuthenticationTypesContext context)
        {
            IDictionary<string, ClaimsIdentity> identity = new Dictionary<string, ClaimsIdentity>();
            AuthenticationResults authenticationResult = AuthenticationResults.FailInvalidCredentials;
            var user = await this._manager.FindAsync(context.UserName, context.Password);
            if(user != null)
            {
                authenticationResult = AuthenticationResults.Success;
                identity = await this._claimsProvider.GenerateUserIdentitiesAsync(user, context.AuthenticationTypes);
            }
            return new AuthenticationResult(authenticationResult, identity);
        }
        public async Task<UserInfoResult> FindUserByEmail(string email)
        {
            var user = await this._manager.FindByEmailAsync(email);
            if (user == null)
                return null;
            return new UserInfoResult(user.Id, user.Email);
        }

        public async Task<string> GeneratePasswordResetToken(string userId)
        {
            return await this._manager.GeneratePasswordResetTokenAsync(userId);
        }

        public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                var user = await this._manager.FindByIdAsync(userId);
                var result = await this._manager.ResetPasswordAsync(userId, token, newPassword);
                return result.Succeeded;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> ValidateResetPasswordTokenAsync(string userId, string token)
        {
            var protector = this._manager.UserTokenProvider;
            var user = await this._manager.FindByIdAsync(userId);
            if (user == null)
                return false;
            var isValid = await protector.ValidateAsync("ResetPassword", token, this._manager, user);
            return isValid;
        }
    }
}