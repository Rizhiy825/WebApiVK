using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net;
using WebApiVK.Interfaces;

namespace WebApiVK.Authorization;

public class BasicAuthenticationEvents
{
    public Func<HttpContext, Task> OnChallenge { get; set; }

    public async Task ChallengeAsync(HttpContext context)
    {
        await OnChallenge(context);
    }
}
