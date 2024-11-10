namespace BreadAPI.Services.Users
{
    public enum UserAuthType
    {
        admin = 'A',
        baker = 'B',
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
}
