using Backend.Services;
using Backend.Services.Users;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly UsersAuthenticationService _usersAuthenticationService;
        private readonly UsersAuthorazationService _usersAuthorazationService;
        public UsersController(
            UsersService usersService, 
            UsersAuthenticationService usersAuthenticationService,
            UsersAuthorazationService usersAuthorizationService)
        {
            _usersService = usersService;
            _usersAuthenticationService = usersAuthenticationService;
            _usersAuthorazationService = usersAuthorizationService;
        }

        [HttpGet("{uuid}")]
        public async Task<User> GetUser(string uuid)
        {
            return await _usersService.GetUser(uuid);
        }

        [HttpGet("auth/{uuid}")]
        public async Task<List<char>> GetUserAuthLevels(string uuid)
        {
            await _usersAuthenticationService.AuthenticateUser(Request.Headers);
            _usersAuthorazationService.IsUserRequestedUser(Request.Headers, uuid);

            UserAuth auth = await _usersAuthorazationService.GetUserAuth(uuid);
            return auth.Acces.Select(x => (char)x).ToList();
        }

        // Put: api/<UsersController>
        [HttpPost("create")]
        public async Task<LoginReponse> AddUser([FromBody] UserPut user)
        {
            return new LoginReponse() { token = await _usersService.AddUser(user, new UserAuthType[] {UserAuthType.user}) };
        }

        // Post: api/<UsersController>/login
        [HttpPost("token")]
        public async Task<LoginReponse> login([FromBody] LoginRequest loginData)
        {
            return new LoginReponse()
            {
                token = await _usersAuthenticationService.login(loginData.email, loginData.password)
            };
        }

        [HttpGet("is_token_correct")]
        public async Task<bool> auth()
        {
            try
            {
                await _usersAuthenticationService.AuthenticateUser(Request.Headers);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        [HttpPut("language/{lang}/{uuid}")]
        public async Task UpdateLanguage(string lang, string uuid)
        {
            await _usersAuthenticationService.AuthenticateUser(Request.Headers);
            _usersAuthorazationService.IsUserRequestedUser(Request.Headers, uuid);

            await _usersService.UpdateUserLanguage(uuid, lang);
        }

        [HttpPut("request_password_reset/{email}")]
        public async Task ResetPasswordRequest(string email)
        {
            User user = await _usersService.GetUserByEmail(email);

            await _usersService.RequestResetUserPassword(user.UUID);
        }

        [HttpPost("request_password_reset/correct_code/{code}/{email}")]
        public async Task<bool> ResetPasswordCodeCorrect(int code, string email)
        {
            User user = await _usersService.GetUserByEmail(email);

            return await _usersService.IsUserCodeCorrect(user.UUID, code);
        }

        [HttpPut("request_password_reset/reset/{email}")]
        public async Task ResetPassword([FromBody] ResetPasswordRequest request,string email)
        {
            User user = await _usersService.GetUserByEmail(email);

            await _usersService.ResetUserPassword(user.UUID, request.code, request.password);
        }
    }
}
