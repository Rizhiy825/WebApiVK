using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApiVK.Authorization;
using WebApiVK.Interfaces;

namespace Tests;

public class TestableBasicAuthenticationHandler : BasicAuthenticationHandler
{
    public TestableBasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService,
        ICoder coder)
        : base(options, logger, encoder, clock, userService, coder)
    {
    }

    public new async Task<AuthenticateResult> HandleAuthenticateAsync() => 
        await base.HandleAuthenticateAsync();
}