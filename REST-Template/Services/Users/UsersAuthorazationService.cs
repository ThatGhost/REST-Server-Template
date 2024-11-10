using Backend.Services.Users;

namespace Backend.Services
{
    public class UsersAuthorazationService: IUsersAuthorazationService
    {
        private readonly IUsersRepository _usersRepository;
        public UsersAuthorazationService(IUsersRepository usersService)
        {
            _usersRepository = usersService;
        }

        public void IsUserRequestedUser(IHeaderDictionary headers, string uuid)
        {
            string tokenid = GetUserId(headers);
            if (tokenid != uuid) throw new UnauthorizedAccessException("user not authorized");
        }

        public string GetUserId(IHeaderDictionary headers)
        {
            const string authHeaderKey = "Authentication";
            if (!headers.ContainsKey(authHeaderKey)) throw new UnauthorizedAccessException("No authentication header found");
            string token = headers[authHeaderKey]!;
            return token.Substring(0, token.IndexOf(":"));
        }

        public async Task<UserAuth> GetUserAuth(string uuid)
        {
            return await _usersRepository.GetUserAuthData(uuid);
        }

        public async Task IsUserAuthorizedOnAccess(string uuid, UserAuthType acces)
        {
            UserAuth user = await _usersRepository.GetUserAuthData(uuid);
            if (!user.Acces.Contains(acces)) throw new UnauthorizedAccessException("user not authorized");
        }

        public async Task IsUserAuthorizedOnAccess(string uuid, UserAuthType[] acces)
        {
            UserAuth user = await _usersRepository.GetUserAuthData(uuid);
            bool isAuth = false;
            foreach (var auth in acces)
            {
                if(user.Acces.Contains(auth))
                {
                    isAuth = true;
                    break;
                }
            }
            if (!isAuth) throw new UnauthorizedAccessException("user not authorized");
        }
    }
}
