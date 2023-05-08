using WebApiVK.Authorization;
using WebApiVK.Models;

namespace WebApiVK.Interfaces;

public interface IUserService
{
    Task<UserToAuth> Authenticate(string username, string password);
}