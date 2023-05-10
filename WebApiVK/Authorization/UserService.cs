using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApiVK.Domain;
using WebApiVK.Interfaces;

namespace WebApiVK.Authorization;

// TODO разберись с методом Authenticate
public class UserService : IUserService
{
    private IEncryptor encryptor;
    private readonly IUsersRepository repository;

    public UserService(IEncryptor encryptor, IUsersRepository repository)
    {
        this.encryptor = encryptor;
        this.repository = repository;
    }

    public async Task<UserToAuth> AuthenticateAdmin(string username, string password)
    {
        var user = await repository.FindByLogin(username);

        if (user == null || user.Group.Code != GroupType.Admin) return null;

        var passwordHash = encryptor.EncryptPassword(password);

        if (passwordHash == user.Password)
        {
            return await Task.FromResult(new UserToAuth(user.Id, username, user.Group));
        }
        else
        {
            return null;
        }
    }
}