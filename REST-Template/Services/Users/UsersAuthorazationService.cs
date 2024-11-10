using System.Linq;

using Backend.Services.Users;

namespace Backend.Services
{
    public class UsersAuthorazationService
    {
        private readonly UsersRepository _usersRepository;
        public UsersAuthorazationService(UsersRepository usersService)
        {
            _usersRepository = usersService;
        }

        public void isUserRequestedUser(IHeaderDictionary headers, string uuid)
        {
            string tokenid = getUserId(headers);
            if (tokenid != uuid) throw new UnauthorizedAccessException("user not authorized");
        }

        public string getUserId(IHeaderDictionary headers)
        {
            const string authHeaderKey = "Authentication";
            if (!headers.ContainsKey(authHeaderKey)) throw new UnauthorizedAccessException("No authentication header found");
            string token = headers[authHeaderKey]!;
            return token.Substring(0, token.IndexOf(":"));
        }

        public async Task<UserAuth> GetUserAuth(string uuid)
        {
            return await _usersRepository.getUserAuthData(uuid);
        }

        public async Task isUserAuthorizedOnAcces(string uuid, UserAuthType acces)
        {
            UserAuth user = await _usersRepository.getUserAuthData(uuid);
            if (!user.Acces.Contains(acces)) throw new UnauthorizedAccessException("user not authorized");
        }

        public async Task isUserAuthorizedOnAcces(string uuid, UserAuthType[] acces)
        {
            UserAuth user = await _usersRepository.getUserAuthData(uuid);
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
