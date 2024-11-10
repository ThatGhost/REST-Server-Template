using Backend.Controllers;
using Backend.Services.Core;
using Backend.core;

namespace Backend.Services.Users
{
    public class UsersService: IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersAuthenticationService _usersAuthenticationService;
        private readonly IFileService _fileService;

        public UsersService(
            IUsersRepository usersRepository,
            IUsersAuthenticationService usersAuthenticationService,
            IFileService fileService)
        {
            _usersRepository = usersRepository;
            _usersAuthenticationService = usersAuthenticationService;
            _fileService = fileService;
        }

        public async Task<string> AddUser(UserPut newUser, UserAuthType[] auth)
        {
            if (!IsValidEmail(newUser.Email)) throw new BadRequestExeption("not a valid email");
            if (newUser.Password.Length <= 5) throw new BadRequestExeption("password not long enough");

            newUser.Email = newUser.Email.ToLower();
            newUser.Password = _usersAuthenticationService.GetHashString(newUser.Password);

            return await _usersRepository.AddUser(newUser, auth);
        }

        public async Task<User> GetUser(string uuid)
        {
            return await _usersRepository.GetUser(uuid);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _usersRepository.GetUserByEmail(email.ToLower());
        }

        public async Task UpdateUserLanguage(string uuid, string language)
        {
            string[] acceptedLanguages = new string[] { "en_US", "nl_NL" };
            if (!acceptedLanguages.Contains(language)) throw new BadRequestExeption("language not correct");

            await _usersRepository.UpdateUsersLanguage(language, uuid);
        }

        // TODO: Send Email with prefered API to get the code to the user
        public async Task RequestResetUserPassword(string uuid)
        {
            User user = await _usersRepository.GetUser(uuid);

            Random rnd = new Random();
            int code = rnd.Next(999999);

            await _usersRepository.AddPasswordResetRequest(uuid, code);
        }

        public async Task<bool> IsUserCodeCorrect(string uuid, int code)
        {
            int userCode = await _usersRepository.GetPasswordResetRequest(uuid);
            return code == userCode; 
        }

        public async Task ResetUserPassword(string uuid, int code, string password)
        {
            int userCode = await _usersRepository.GetPasswordResetRequest(uuid);
            if(code != userCode) throw new ForbiddenExeption("code not correct");

            await _usersRepository.UpdateUserPassword(uuid, _usersAuthenticationService.GetHashString(password));
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.ToLower().Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
