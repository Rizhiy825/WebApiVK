using WebApiVK.Models;

namespace WebApiVK.Authorization;

public interface IUserService
{
    Task<UserToAuthDto> Authenticate(string username, string password);
}