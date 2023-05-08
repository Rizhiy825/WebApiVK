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

    public async Task<UserToAuth> Authenticate(string username, string password)
    {
        var user = repository.FindByLogin(username);

        if (user == null)
        {
            return await Task.FromResult<UserToAuth>(null);
        }

        if (user.Group.Code == GroupType.Admin)
        {
            var passwordHash = encryptor.EncryptPassword(password);

            if (passwordHash == user.Password)
            {
                return await Task.FromResult(new UserToAuth(user.Id, username, user.Group));
            }
        }

        return await Task.FromResult(new UserToAuth());
    }
}