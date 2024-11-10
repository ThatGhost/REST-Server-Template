using Backend.Services;
using Backend.Services.Users;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests
{
    public class UserAuthenticationServiceTests
    {
        private Mock<IUsersRepository> _usersRepositoryMock = new Mock<IUsersRepository>();
        private UsersAuthenticationService _service;
        private const string testUuid = "test-uuid";
        private UserAuth mockUserAuth = new UserAuth
        {
            Email = "test@example.com",
            Password = "password",
            UUID = testUuid,
            Acces = new UserAuthType[] { UserAuthType.user }
        };

        [SetUp]
        public void Setup()
        {
            _service = new UsersAuthenticationService(_usersRepositoryMock.Object);
        }

        [Test]
        public void AuthenticateUser_ShouldThrow_NoHeader()
        {
            // Arrange
            IHeaderDictionary headers = new HeaderDictionary();

            // Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>( async () =>
            {
                await _service.AuthenticateUser(headers);
            });
        }

        [Test]
        public void AuthenticateUser_ShouldThrow_NotCorrectToken()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthData("a")).ReturnsAsync(mockUserAuth);
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Append("Authentication","a:b");

            // Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.AuthenticateUser(headers);
            });
        }

        [Test]
        public async Task AuthenticateUser_ShouldSucceed_CorrectToken()
        {
            // Arrange
            UserAuth userAuthHashed = mockUserAuth;
            userAuthHashed.Password = _service.GetHashString(userAuthHashed.Password);

            _usersRepositoryMock.Setup(u => u.GetUserAuthDataByEmail(mockUserAuth.Email)).ReturnsAsync(userAuthHashed);

            string correctToken = await _service.login(mockUserAuth.Email, mockUserAuth.Password);
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Append("Authentication", $"{correctToken}");

            _usersRepositoryMock.Setup(u => u.GetUserAuthData(mockUserAuth.UUID)).ReturnsAsync(userAuthHashed);

            // Assert
            Assert.DoesNotThrowAsync(async () => { 
                await _service.AuthenticateUser(headers);
            });
        }

        [Test]
        public void login_ShouldThrow_PasswordNotCorrect()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthDataByEmail(mockUserAuth.Email)).ReturnsAsync(mockUserAuth);

            // Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.login(mockUserAuth.Email, "DiffPassword");
            });
        }

        [Test]
        public async Task login_ShouldPass_PasswordCorrect()
        {
            // Arrange
            UserAuth userAuthHashed = mockUserAuth;
            userAuthHashed.Password = _service.GetHashString(userAuthHashed.Password);
            _usersRepositoryMock.Setup(u => u.GetUserAuthDataByEmail(mockUserAuth.Email)).ReturnsAsync(userAuthHashed);

            // Act
            string token = await _service.login(mockUserAuth.Email, mockUserAuth.Password);
            
            // Assert
            Assert.That(token, Is.EqualTo($"{mockUserAuth.UUID}:{_service.GetHashString(userAuthHashed.Email + userAuthHashed.Password)}"));
        }

        [Test]
        public void GetHashString_ShouldPass()
        {
            // Arrange
            const string hashed = "45FC0697754E42B64043F0FFC50EEDDE419C0DFD478F20602576DC4841A6ABCF";
            
            // Act
            string hash = _service.GetHashString("SOMETHING TO HASH");
            
            // Assert
            Assert.That(hash, Is.EqualTo(hashed));
        }
    }
}