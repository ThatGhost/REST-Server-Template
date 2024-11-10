using Backend.Controllers;

namespace Backend.Services.Users
{
    public enum UserAuthType
    {
        admin = 'A',
        user = 'U',
    }

    public struct User
    {
        public string Email { get; set; }
        public string UUID { get; set; }
        public string Language { get; set; }
    }

    public struct UserAuth
    {
        public string Email { get; set; }

        public string Password { get; set; }
        public string UUID { get; set; }
        public UserAuthType[] Acces { get; set; }
    }

    public interface IUsersAuthenticationService
    {
        public Task AuthenticateUser(IHeaderDictionary headers);
        public Task<string> login(string email, string password);
        public string GetHashString(string inputString);
    }

    public interface IUsersAuthorazationService
    {
        void IsUserRequestedUser(IHeaderDictionary headers, string uuid);
        string GetUserId(IHeaderDictionary headers);
        Task<UserAuth> GetUserAuth(string uuid);
        Task IsUserAuthorizedOnAccess(string uuid, UserAuthType access);
        Task IsUserAuthorizedOnAccess(string uuid, UserAuthType[] access);
    }

    public interface IUsersRepository
    {
        public Task<User> GetUser(string uuid);
        public Task<User> GetUserByEmail(string email);
        public Task<UserAuth> GetUserAuthData(string uuid);
        public Task<UserAuth> GetUserAuthDataByEmail(string email);
        public Task<string> AddUser(UserPut user, UserAuthType[] acces);
        public Task UpdateUsersLanguage(string language, string uuid);
        public Task AddPasswordResetRequest(string uuid, int randomNumber);
        public Task<int> GetPasswordResetRequest(string uuid);
        public Task UpdateUserPassword(string uuid, string password);
    }

    public interface IUsersService
    {
        Task<string> AddUser(UserPut newUser, UserAuthType[] auth);
        Task<User> GetUser(string uuid);
        Task<User> GetUserByEmail(string email);
        Task UpdateUserLanguage(string uuid, string language);
        Task RequestResetUserPassword(string uuid);
        Task<bool> IsUserCodeCorrect(string uuid, int code);
        Task ResetUserPassword(string uuid, int code, string password);
    }

}
