namespace WebApiVK.Interfaces;

public interface IAuthEvents
{
    public Func<HttpContext, Task> OnChallenge { get; set; }

    public Task ChallengeAsync(HttpContext context);
}