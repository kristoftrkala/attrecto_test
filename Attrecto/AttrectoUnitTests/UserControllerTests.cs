using Attrecto;
using Attrecto.Controllers;
using Attrecto.Data;
using Attrecto.Dtos;
using Attrecto.Dtos.User;
using Attrecto.IdentityServer;
using Attrecto.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AttrectoUnitTests
{
    public class UserControllerTests
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IClaimsHelper> claimHelperMock;
        private IMapper mapper;

        [SetUp]
        public void Setup()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            claimHelperMock = new Mock<IClaimsHelper>();
            mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            }).CreateMapper();
        }

        [Test]
        public async Task GetUserByIdAsync_WithNotExistingUser_ReturnsNull()
        {
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);
            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Basic");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);

            var result = await controller.GetUserById(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUserByIdAsync_WithExistingUser_ReturnsUserData()
        {
            var expectedItem = new User
            {
                IdUser = 1,
                Email = "unit@test.com",
                Password = "1"
            };

            userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedItem);

            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);
            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Basic");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);

            var result = await controller.GetUserById(1);

            Assert.That(result, Is.TypeOf<GetUserDto>());
        }

        [Test]
        public void GetUserByIdAsync_WithExistingUserUnauthorized_ThrowsUnauthorized()
        {
            var unaccessableUser = new User
            {
                IdUser = 2,
                Email = "unit@test.com",
                Password = "1"
            };

            userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(unaccessableUser);

            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);
            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Basic");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await controller.GetUserById(2));
        }

        [Test]
        public void GetAllUsersAsync_WithBasicUser_ThrowsUnauthorized()
        {
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            claimHelperMock.Setup(x => x.GetIdFromClaim(It.IsAny<HttpContext>()))
                .Returns(1);
            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Basic");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await controller.GetAllUser());
        }

        [Test]
        public async Task GetAllUsersAsync_WithAdminUser_ReturnsAllUser()
        {
            var expectedResult = new List<User>
            {
                new User
                {
                    IdUser = 1,
                    Email = "first@test.com",
                    Password = "1"
                },
                new User
                {
                    IdUser = 2,
                    Email = "second@test.com",
                    Password = "1"
                },
            };
            userRepositoryMock.Setup(x => x.GetAllUserAsync())
                .ReturnsAsync(expectedResult);

            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Admin");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);

            var result = await controller.GetAllUser();

            result.Should().BeEquivalentTo(result,
                options => options.ComparingByMembers<GetUserDto>().ExcludingMissingMembers());
        }

        [Test]
        public async Task AddUsersAsync_WithAdminUser_ReturnsAddedUser()
        {
            var addUserDto = new AddUserDto
            {
                Email = "add@test.com",
                Name = "Add Test"
            };
            var expectedResult = new User
            {
                IdUser = 1,
                Email = "add@test.com",
                Password = "1",
                Name = "Add Test"
            };
            userRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(expectedResult);

            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Admin");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);

            var result = await controller.AddUserFromAdmin(addUserDto);

            result.Should().BeEquivalentTo(result,
                options => options.ComparingByMembers<GetUserDto>().ExcludingMissingMembers());
        }

        [Test]
        public async Task UpdateUserAsync_WithAdminUser_ReturnsAddedUser()
        {
            var existingUser = new User
            {
                IdUser = 1,
                Email = "first@test.com",
                Password = "1"
            };
            var updateUserDto = new UpdateUserDto
            {
                IdUser = 1,
                Email = "update@test.com",
                Name = "Update Test",
            };
            userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingUser);
            userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>()));

            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Admin");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);

            var result = await controller.UpdateUser(1, updateUserDto);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task DeleteUserAsync_WithAdminUser_RemovesUser()
        {
            var existingUser = new User
            {
                IdUser = 1,
                Email = "test@test.com",
                Password = "1"
            };

            userRepositoryMock.Setup(x => x.DeleteUser(It.IsAny<int>()));

            claimHelperMock.Setup(x => x.GetRoleFromClaim(It.IsAny<HttpContext>()))
                .Returns("Admin");

            var controller = new UserController(userRepositoryMock.Object, mapper, claimHelperMock.Object);
            var result = await controller.DeleteUser(existingUser.IdUser);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}