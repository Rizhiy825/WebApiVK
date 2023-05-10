using WebApiVK.Authorization;

namespace WebApiVK.Interfaces;

public interface IUserService
{
    Task<UserToAuth> AuthenticateUser(string username, string password);
}