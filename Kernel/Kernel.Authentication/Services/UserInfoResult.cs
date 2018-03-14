namespace Kernel.Authentication.Services
{
    public class UserInfoResult
    {
        public UserInfoResult(string userId, string userEmail)
        {
            this.UserEmail = userEmail;
            this.UserId = userId;
        }

        public string UserId { get; }
        public string UserEmail { get; }
    }
}