using Microsoft.EntityFrameworkCore;
using WebApiVK.Interfaces;

namespace WebApiVK.Domain;

public class TestRepository : IUsersRepository
{
    private readonly TestContext context;
    public TestRepository(TestContext context)
    {
        this.context = context;
    }
    public UserEntity FindById(Guid id)
    {
        var user = context.Users.Find(id);
        return user;
    }

    public UserEntity FindByLogin(string login)
    {
        var user = context.Users.Include(x => x.Group)
            .Where(x => x.Login == login).First();
        return user;
    }

    public UserEntity Insert(UserEntity userEntity)
    {
        context.Add(userEntity);
        context.SaveChanges();
        return userEntity;
    }
}