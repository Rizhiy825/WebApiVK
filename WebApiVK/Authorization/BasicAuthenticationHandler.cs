using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WebApiVK.Interfaces;
using WebApiVK.Authorization;

namespace WebApiVK.Authorization;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService userService;
    private readonly ICoder coder;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService,
        ICoder coder)
        : base(options, logger, encoder, clock)
    {
        this.userService = userService;
        this.coder = coder;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        UserToAuth user;

        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentials = coder.DecodeCredentials(authHeader.Parameter);
        var username = credentials.Item1;
        var password = credentials.Item2;
        user = await userService.Authenticate(username, password);

        if (user == null)
            return AuthenticateResult.Fail("Invalid Credentials");

        if (user.IsEmpty())
            return AuthenticateResult.Fail("User without rights");

        // В случае прохождения аутентификации выдаем Claim
        var claims = new[]
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("Login", user.Login),
            new Claim(ClaimTypes.Role, "admin")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}