using WebApiVK.Interfaces;

namespace WebApiVK.Authorization;

public class UserService : IUserService
{
    private IEncryptor encryptor;
    private readonly IUsersRepository repository;

    public UserService(IEncryptor encryptor, IUsersRepository repository)
    {
        this.encryptor = encryptor;
        this.repository = repository;
    }

    public async Task<UserToAuth> AuthenticateUser(string username, string password)
    {
        var user = await repository.FindByLogin(username);

        if (user == null) return null;

        var passwordHash = encryptor.EncryptPassword(password);

        if (passwordHash == user.Password)
        {
            return await Task.FromResult(new UserToAuth(user.Id, username, user.Group));
        }
        
        return null;
    }
}