using WebApiVK.Models;

namespace WebApiVK.Domain;

public interface IUsersRepository
{
    public User GetUserById(Guid id);
}