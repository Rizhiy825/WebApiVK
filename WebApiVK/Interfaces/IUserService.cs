using WebApiVK.Authorization;
using WebApiVK.Models;

namespace WebApiVK.Interfaces;

public interface IUserService
{
    Task<UserToAuth> AuthenticateUser(string username, string password);
}