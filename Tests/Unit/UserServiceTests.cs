using FakeItEasy;
using FluentAssertions;
using WebApiVK.Authorization;
using WebApiVK.Domain;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace Tests.Unit;

public class UserServiceTests
{
    [Fact]
    public async void AuthUserThatNotAdmin_ShouldReturnUser()
    {
        var encryptor = A.Fake<IEncryptor>();
        var fakeRepo = A.Fake<IUsersRepository>();
        var service = new UserService(encryptor, fakeRepo);

        var login = "login";
        var password = "password";

        A.CallTo(() => fakeRepo.FindByLogin(login))
            .Returns(new UserEntity(
                new Guid(),
                login,
                DateTime.MinValue,
                new UserGroup(GroupType.User, "Description"),
                new UserState(StateType.Active, "Active"), password));

        A.CallTo(() => encryptor.EncryptPassword(password))
            .Returns(password);

        var authUser = await service.AuthenticateUser(login, password);
        
        authUser.Should()
            .BeEquivalentTo(new UserToAuth(
                authUser.Id,
                login,
                new UserGroup(GroupType.User, "Description"),
                new UserState(StateType.Active, "Active")));
    }

    [Fact]
    public async void AuthAdminWithWrongPassword_ShouldReturnNull()
    {
        var encryptor = A.Fake<IEncryptor>();
        var fakeRepo = A.Fake<IUsersRepository>();
        var service = new UserService(encryptor, fakeRepo);

        var login = "login";
        var password = "password";
        var wrongPassword = "wrong password";

        A.CallTo(() => fakeRepo.FindByLogin(login))
            .Returns(new UserEntity(
                new Guid(),
                login,
                DateTime.MinValue,
                new UserGroup(GroupType.Admin, "Description"),
                new UserState(StateType.Active, "Active"), password));

        A.CallTo(() => encryptor.EncryptPassword(password))
            .Returns(wrongPassword);

        var authUser = await service.AuthenticateUser(login, password);

        authUser.Should().BeNull();
    }

    [Fact]
    public async void AuthAdminWithCorrectPassword_ShouldReturnAdminUser()
    {
        var encryptor = A.Fake<IEncryptor>();
        var fakeRepo = A.Fake<IUsersRepository>();
        var service = new UserService(encryptor, fakeRepo);

        var login = "login";
        var password = "password";

        A.CallTo(() => fakeRepo.FindByLogin(login))
            .Returns(new UserEntity(
                new Guid(),
                login,
                DateTime.MinValue,
                new UserGroup(GroupType.Admin, "Description"),
                new UserState(StateType.Active, "Active"), password));

        A.CallTo(() => encryptor.EncryptPassword(password))
            .Returns(password);

        var authUser = await service.AuthenticateUser(login, password);

        authUser.Should()
            .BeEquivalentTo(new UserToAuth(
                authUser.Id,
                login,
                new UserGroup(GroupType.Admin, "Description"),
                new UserState(StateType.Active, "Active")));
    }
}