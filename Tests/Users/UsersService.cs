using Backend.Controllers;
using Backend.core;
using Backend.Services.Core;
using Backend.Services.Users;
using Moq;

namespace Tests.Users
{
    public class UsersServiceTests
    {
        private Mock<IUsersRepository> _usersRepositoryMock = new Mock<IUsersRepository>();
        private Mock<IUsersAuthenticationService> _usersAuthenticationServiceMock = new Mock<IUsersAuthenticationService>();
        private Mock<FileService> _fileServiceMock = new Mock<FileService>();
        private UsersService _service;

        private UserPut newUserMock = new UserPut()
        {
            Email = "test@email.com",
            Password = "Password1234",
        };

        private User userMock = new User()
        {
            Email = "user@email.com",
            Language = "en_US",
            UUID = "SOMEUUID"
        };

        [SetUp]
        public void Setup()
        {
            _fileServiceMock.Reset();
            _usersAuthenticationServiceMock.Reset();
            _usersRepositoryMock.Reset();

            _service = new UsersService(_usersRepositoryMock.Object, _usersAuthenticationServiceMock.Object, _fileServiceMock.Object);
        }

        [Test]
        public void AddUser_ShouldThrow_UnvalidEmail()
        {
            // Arrange
            UserPut userPutBadEmail = newUserMock;
            userPutBadEmail.Email = String.Empty;

            // Act & Assert
            Assert.ThrowsAsync<BadRequestExeption>(async () =>
            {
                await _service.AddUser(userPutBadEmail, new UserAuthType[0]);
            }, "not a valid email");
        }

        [Test]
        public void AddUser_ShouldThrow_UnvalidPassword()
        {
            // Arrange
            UserPut userPutBadPassword = newUserMock;
            userPutBadPassword.Password = String.Empty;

            // Act & Assert
            Assert.ThrowsAsync<BadRequestExeption>(async () =>
            {
                await _service.AddUser(userPutBadPassword, new UserAuthType[0]);
            }, "password not long enough");
        }

        [Test]
        public async Task AddUser_ShouldReturnUUID_validParameters()
        {
            // Arrange
            string newUUID = "someUUID";
            UserAuthType[] userAuthTypes = new UserAuthType[] { UserAuthType.user };
            _usersRepositoryMock.Setup(u => u.AddUser(newUserMock, userAuthTypes)).ReturnsAsync(newUUID);
            
            // Act
            string uuid = await _service.AddUser(newUserMock, userAuthTypes);

            // Assert
            Assert.That(uuid, Is.EqualTo(newUUID));
        }

        [Test]
        public async Task GetUser_ShouldReturnUser()
        {
            // Arrrange
            _usersRepositoryMock.Setup(u => u.GetUser(userMock.UUID)).ReturnsAsync(userMock);

            // Act
            User user = await _service.GetUser(userMock.UUID);

            // Assert
            Assert.That(user, Is.EqualTo(userMock));
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnUser()
        {
            // Arrrange
            _usersRepositoryMock.Setup(u => u.GetUserByEmail(userMock.Email)).ReturnsAsync(userMock);

            // Act
            User user = await _service.GetUserByEmail(userMock.Email);

            // Assert
            Assert.That(user, Is.EqualTo(userMock));
        }

        [Test]
        public void UpdateUserLanguage_shouldThrow_NotAcceptedLanguage()
        {
            // Arrange
            string badLanguage = "de_DE";

            // Act & Assert
            Assert.ThrowsAsync<BadRequestExeption>(async () =>
            {
                await _service.UpdateUserLanguage(userMock.UUID, badLanguage);
            }, "language not correct");
        }

        [Test]
        public async Task UpdateUserLanguage_shouldUpdateLanguage()
        {
            // Arrange
            string newLanguage = "en_US";

            // Act
            await _service.UpdateUserLanguage(userMock.UUID, newLanguage);

            // Assert
            _usersRepositoryMock.Verify(u => u.UpdateUsersLanguage(newLanguage, userMock.UUID), Times.Once);
        }

        [Test]
        public async Task RequestResetUserPassword_ShouldAddARequest()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUser(userMock.UUID)).ReturnsAsync(userMock);

            // Act
            await _service.RequestResetUserPassword(userMock.UUID);
            
            // Assert
            _usersRepositoryMock.Verify(u => u.AddPasswordResetRequest(userMock.UUID, It.IsInRange<int>(0, 999999, Moq.Range.Inclusive)), Times.Once);
        }

        [Test]
        public async Task IsUserCodeCorrect_shouldReturnTrue_WithCorrectCode()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetPasswordResetRequest(userMock.UUID)).ReturnsAsync(50);

            // Act
            bool isCorrect = await _service.IsUserCodeCorrect(userMock.UUID, 50);

            // Assert
            Assert.IsTrue(isCorrect);
        }

        [Test]
        public void ResetUserPassword_ShouldThrow_IncorrectCode()
        {
            // Arrange
            _usersRepositoryMock.Reset();
            _usersRepositoryMock.Setup(u => u.GetPasswordResetRequest(userMock.UUID)).ReturnsAsync(50);

            // Act & Assert
            Assert.ThrowsAsync<ForbiddenExeption>(async () =>
            {
                await _service.ResetUserPassword(userMock.UUID, 1312, "newPassword");
            });

            _usersRepositoryMock.Verify(u => u.UpdateUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ResetUserPassword_ShouldResetPassword_CorrectCode()
        {
            // Arrange
            _usersRepositoryMock.Reset();
            const string newPassword = "newPassword";
            _usersRepositoryMock.Setup(u => u.GetPasswordResetRequest(userMock.UUID)).ReturnsAsync(50);

            // Act
            await _service.ResetUserPassword(userMock.UUID, 50, newPassword);

            // Assert
            _usersRepositoryMock.Verify(u => u.UpdateUserPassword(userMock.UUID, It.IsAny<string>()), Times.Once);
            _usersAuthenticationServiceMock.Verify(u => u.GetHashString(newPassword), Times.Once);
        }
    }
}
