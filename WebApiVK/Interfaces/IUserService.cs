using WebApiVK.Models;

namespace WebApiVK.Interfaces;

public interface IUserService
{
    Task<UserToAuthDto> Authenticate(string username, string password);
}