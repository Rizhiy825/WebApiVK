using WebApiVK.Authorization;
using WebApiVK.Models;

namespace WebApiVK.Interfaces;

public interface IUserService
{
    Task<UserToAuth> AuthenticateAdmin(string username, string password);
}