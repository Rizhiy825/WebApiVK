using WebApiVK.Domain;

namespace WebApiVK.Interfaces;

public interface IUsersRepository
{
    public UserEntity GetUserById(Guid id);
    public UserEntity GetUserByLogin(string login);
    public UserEntity Insert(UserEntity userEntity);
}