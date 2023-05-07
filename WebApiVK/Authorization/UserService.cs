using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace WebApiVK.Authorization;

public class UserService : IUserService
{
    private IEncryptor _encryptor;
    private readonly IUsersRepository _repository;

    public UserService(IEncryptor encryptor, IUsersRepository repository)
    {
        _encryptor = encryptor;
        _repository = repository;
    }

    public Task<UserToAuthDto> Authenticate(string username, string password)
    {
        if (username == "admin")
        {
            var passwordHash = _encryptor.EncryptPassword(password);

            if (passwordHash == "WTgmRoHu2yk6gt1YN44y86vREhV6JLQTFkCZYoLpSgM=")
            {
                return Task<UserToAuthDto>.Run(() => new UserToAuthDto(){Id = new Guid(), Login = "admin"});
            }
        }

        return null;
    }

    
}