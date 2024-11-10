using Backend.Services.Users;
using Backend.Controllers;
using Backend.Services.Core;
using Backend.core;

namespace Backend.Services
{
    public class UsersRepository : BaseRepository, IUsersRepository
    {
        public UsersRepository(IConfiguration config) : base(config)
        {

        }

        public async Task<User> GetUser(string uuid)
        {
            UserGetData? user = await readOne<UserGetData>($"select email, language from users where id = @uuid;", new { uuid });
            if (user == null) throw new NotFoundExeption("User not found");

            return new User()
            {
                Email = user.email,
                UUID = uuid,
                Language = user.language,
            };
        }

        public async Task<User> GetUserByEmail(string email)
        {
            UserGetData? user = await readOne<UserGetData>($"select email, language, id from users where email = @email;", new { email });
            if (user == null) throw new NotFoundExeption("User not found");

            return new User()
            {
                Email = user.email,
                UUID = user.id,
                Language = user.language,
            };
        }


        public async Task<UserAuth> GetUserAuthData(string uuid)
        {
            UserAuthData? userAuthData = await readOne<UserAuthData>($"select password, id, email, acces from users where id = @uuid", new { uuid });
            if (userAuthData == null) throw new NotFoundExeption("User not found");

            return convertToUserAuth(userAuthData);
        }

        public async Task<UserAuth> GetUserAuthDataByEmail(string email)
        {
            UserAuthData? userAuthData = await readOne<UserAuthData>($"select password, id, email, acces from users where email = '{email}'");
            if (userAuthData == null) throw new NotFoundExeption("User not found");

            return convertToUserAuth(userAuthData);
        }

        private UserAuth convertToUserAuth(UserAuthData data)
        {
            return new UserAuth()
            {
                Email = data.email,
                Password = data.password,
                UUID = data.id,
                Acces = data.acces.ToCharArray().Select(a => (UserAuthType)a).ToArray(),
            };
        }

        public async Task<string> AddUser(UserPut user, UserAuthType[] acces)
        {
            string accesString = "";
            foreach (var acc in acces) accesString += (char)acc;
            await write($"INSERT INTO users (id, email, password, acces) VALUES (UUID(), '{user.Email}', '{user.Password}', '{accesString}');");

            var UUID = await readOne<UUIDGet>($"SELECT id FROM users WHERE email = '{user.Email}';");
            if(UUID == null ) throw new Exception("user was not added");
            return UUID.id;
        }

        public async Task UpdateUsersLanguage(string language, string uuid)
        {
            await write(@"
                UPDATE users SET language = @language WHERE id = @uuid;
            ", new {
                language,
                uuid
            });
        }

        public async Task AddPasswordResetRequest(string uuid, int randomNumber)
        {
            await write(@"
                REPLACE INTO password_reset (user_id, code)
                VALUES (@uuid, @code)
            ", new { code = randomNumber, uuid });
        }

        public async Task<int> GetPasswordResetRequest(string uuid)
        {
            UserPasswordReset? userPasswordReset = await readOne<UserPasswordReset>(@"
                SELECT code
                FROM password_reset
                WHERE user_id = @uuid;
            ", new { uuid });

            if (userPasswordReset == null) throw new NotFoundExeption("No password reset requested");
            return userPasswordReset.code;
        }

        public async Task UpdateUserPassword(string uuid, string password)
        {
            await write(@"
                UPDATE users 
                SET password = @password
                WHERE id = @uuid
            ", new
            { uuid, password });
        }

        private class UserGetData
        {
            public required string email { get; set; }
            public required string language { get; set; }
            public required string id { get; set; }
        }

        public class UserAuthData
        {
            public required string email { get; set; }

            public required string password { get; set; }
            public required string acces { get; set; }
            public required string id { get; set; }
        }

        public class UserPasswordReset
        {
            public required int code { get; set; }
        }
    }
}
