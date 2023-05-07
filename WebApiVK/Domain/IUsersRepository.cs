namespace WebApiVK.Domain;

public interface IUsersRepository
{
    public UserEntity GetUserById(Guid id);
}