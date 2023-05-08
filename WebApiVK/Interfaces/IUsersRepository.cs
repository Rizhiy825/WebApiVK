using WebApiVK.Domain;

namespace WebApiVK.Interfaces;

public interface IUsersRepository
{
    public UserEntity FindById(Guid id);
    public UserEntity FindByLogin(string login);
    public UserEntity Insert(UserEntity userEntity);
}