using Microsoft.EntityFrameworkCore;
using WebApiVK.Interfaces;

namespace WebApiVK.Domain;

public class NpgsqlUsersRepository : IUsersRepository
{
    private readonly UsersContext context;

    public NpgsqlUsersRepository(UsersContext context)
    {
        this.context = context;
    }

    public UserEntity FindById(Guid id)
    {
        var user = context.Users.Find(id);
        return user;
    }

    // Добавь валидацию на наличие группы и статуса
    public UserEntity FindByLogin(string login)
    {
        var user = context.Users.Where(x => x.Login == login).First();
        return user;
    }

    public UserEntity Insert(UserEntity user)
    {
        if (user.Group == null)
        {
            user.Group = context.UserGroups.First(x => x.Code == GroupType.User);
        }

        if (user.State == null)
        {
            user.State = context.UserStates.First(x => x.Code == StateType.Active);
        }

        //TODO привязать дату к часовому поясу Москвы
        user.Created = DateTime.UtcNow;

        var added = context.Add(user).Entity;
        context.SaveChanges();
        return added;
    }
}