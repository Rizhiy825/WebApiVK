using FakeItEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using FluentAssertions;
using WebApiVK.Authorization;
using WebApiVK.Interfaces;
using WebApiVK.Domain;
using WebApiVK.Models;

namespace Tests.Unit;

public class BasicAuthenticationHandlerTests
{
    private readonly TestableBasicAuthenticationHandler handler;
    private readonly IUserService userServiceMock;

    public BasicAuthenticationHandlerTests()
    {
        var options = A.Fake<IOptionsMonitor<AuthenticationSchemeOptions>>();
        var loggerFactory = A.Fake<ILoggerFactory>();
        var encoder = A.Fake<UrlEncoder>();
        var clock = A.Fake<ISystemClock>();
        userServiceMock = A.Fake<IUserService>();
        var coder = new Base64Coder();

        handler = new TestableBasicAuthenticationHandler(
            options,
            loggerFactory,
            encoder,
            clock,
            userServiceMock,
            coder);
    }

    [Fact]
    public async Task RequestWithoutHandler_ShouldReturnNoResult()
    {
        var context = new DefaultHttpContext();
        await handler.InitializeAsync(
            new AuthenticationScheme("TestScheme", null, typeof(BasicAuthenticationHandler)),
            context);

        var result = await handler.HandleAuthenticateAsync();

        result.Should().BeEquivalentTo(AuthenticateResult.NoResult());
    }

    [Fact]
    public async Task NotAuthenticatedUser_ShouldReturnFail()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Basic dGVzdDp0ZXN0"; // "test:test" in Base64

        await handler.InitializeAsync(
            new AuthenticationScheme("TestScheme", null, typeof(BasicAuthenticationHandler)),
            context);

        var username = "test";
        var password = "test";

        A.CallTo(() => userServiceMock.AuthenticateUser(username, password))
            .Returns(Task.FromResult<UserToAuth>(null));

        var result = await handler.HandleAuthenticateAsync();

        result.Failure.Should().NotBeNull();
        result.Failure.Message.Should().Be("Invalid username or password");
    }

    [Fact]
    public async Task ValidUserThatInDatabase_ShouldReturnSuccessfulOutcome()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Basic dGVzdDp0ZXN0"; // "test:test" in Base64

        await handler.InitializeAsync(
            new AuthenticationScheme("TestScheme", null, typeof(BasicAuthenticationHandler)),
            context);

        var (username, password) = ("test", "test");
        var userToAuth = new UserToAuth(new Guid(),
            username,
            new UserGroup(GroupType.Admin, "desctiption"),
            new UserState(StateType.Active, "active"));

        A.CallTo(() => userServiceMock.AuthenticateUser(username, password))
            .Returns(Task.FromResult(userToAuth));

        var result = await handler.HandleAuthenticateAsync();

        Assert.True(result.Succeeded);
    }
}