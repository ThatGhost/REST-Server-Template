using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backend.Services;
using Backend.Services.Users;

using Microsoft.AspNetCore.Http;

using Moq;

namespace Tests.Users
{
    public class UsersAuthorazationServiceTests
    {
        private Mock<IUsersRepository> _usersRepositoryMock = new Mock<IUsersRepository>();
        private UsersAuthorazationService _service;
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
            _service = new UsersAuthorazationService(_usersRepositoryMock.Object);
        }

        [Test]
        public void IsUserRequestedUser_ShouldThrow_IsNotRequestedUser()
        {
            // Arrange
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Append("Authentication",$"a:b");

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                _service.IsUserRequestedUser(headers, testUuid);
            });
        }

        [Test]
        public void IsUserRequestedUser_ShouldSucceed_IsRequestedUser()
        {
            // Arrange
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Append("Authentication", $"{testUuid}:b");

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                _service.IsUserRequestedUser(headers, testUuid);
            });
        }

        [Test]
        public void GetUserId_ShouldThrow_NoHeader()
        {
            // Arrange
            IHeaderDictionary headers = new HeaderDictionary();

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                _service.GetUserId(headers);
            });
        }

        [Test]
        public void GetUserId_ShouldSucceed_returnsUserId()
        {
            // Arrange
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Append("Authentication", $"{testUuid}:b");

            // Act
            string id = _service.GetUserId(headers);

            Assert.That(id, Is.EqualTo(testUuid));
        }

        [Test]
        public async Task GetUserAuth_ShouldReturnUserAuthData()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthData(testUuid)).ReturnsAsync(mockUserAuth);

            // Act
            UserAuth userAuth = await _service.GetUserAuth(testUuid);

            // Assert
            Assert.That(userAuth, Is.EqualTo(mockUserAuth));
        }

        [Test]
        public void IsUserAuthorizedOnAccess_ShouldThrow_NotAuthorizedOnAcces()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthData(testUuid)).ReturnsAsync(mockUserAuth);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.IsUserAuthorizedOnAccess(testUuid, UserAuthType.admin);
            });
        }

        [Test]
        public void IsUserAuthorizedOnAccess_ShouldThrow_NotAuthorizedOnAccesArray()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthData(testUuid)).ReturnsAsync(mockUserAuth);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.IsUserAuthorizedOnAccess(testUuid, new UserAuthType[] { UserAuthType.admin  });
            });
        }

        [Test]
        public void IsUserAuthorizedOnAccess_ShouldSucceed_AuthorizedOnAcces()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthData(testUuid)).ReturnsAsync(mockUserAuth);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await _service.IsUserAuthorizedOnAccess(testUuid, UserAuthType.user);
            });
        }

        [Test]
        public void IsUserAuthorizedOnAccess_ShouldSucceed_NotAuthorizedOnAccesArray()
        {
            // Arrange
            _usersRepositoryMock.Setup(u => u.GetUserAuthData(testUuid)).ReturnsAsync(mockUserAuth);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await _service.IsUserAuthorizedOnAccess(testUuid, new UserAuthType[] { UserAuthType.user });
            });
        }
    }
}
