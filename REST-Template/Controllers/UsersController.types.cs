namespace BreadAPI.Controllers
{
    public class UserPut
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string email { get; set; }
        public required string password { get; set; }
    }

    public class LoginReponse
    {
        public required string token { get; set; }
    }

    public class ResetPasswordRequest
    {
        public required string password { get; set; }
        public required int code { get; set; }
    }
}
