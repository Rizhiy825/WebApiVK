using WebApiVK.Models;

namespace WebApiVK.Authorization;

public interface IUserService
{
    Task<UserForAuthDTO> Authenticate(string username, string password);
}