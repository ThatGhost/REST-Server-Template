using BreadAPI.Services.Users;

using System.Security.Cryptography;
using System.Text;

namespace BreadAPI.Services
{
    public class UsersAuthenticationService
    {
        private readonly UsersRepository _usersRepository;
        public UsersAuthenticationService(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task AuthenticateUser(IHeaderDictionary headers)
        {
            const string authHeaderKey = "Authentication";
            if (!headers.ContainsKey(authHeaderKey)) throw new UnauthorizedAccessException("No authentication header found");
            string token = headers[authHeaderKey]!;

            if (!await compareLoginHash(token)) throw new UnauthorizedAccessException("Authentication failed");
        }

        private async Task<bool> compareLoginHash(string incomingToken)
        {
            string uuid = incomingToken.Substring(0, incomingToken.IndexOf(":"));
            UserAuth userAuthData = await _usersRepository.getUserAuthData(uuid);

            string correctUserAuthToken = GetHashString(userAuthData.Email + userAuthData.Password);
            string incomingUserAuthToken = incomingToken.Substring(incomingToken.IndexOf(':') + 1);

            return correctUserAuthToken == incomingUserAuthToken;
        }

        public async Task<string> login(string email, string password)
        {
            UserAuth userAuthData = await _usersRepository.getUserAuthDataByEmail(email);
            string newPasswordHash = GetHashString(password);

            if (newPasswordHash != userAuthData.Password) throw new UnauthorizedAccessException("password incorrect");
            return userAuthData.UUID.ToString() + ":" + GetHashString(email + newPasswordHash);
        }

        public string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            string hash = sb.ToString();
            hash += hash.Substring(5, 6);
            hash = hash.Substring(7, 8);
            hash += hash.Substring(2, 2);

            return sb.ToString();
        }

        private byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

    }
}
