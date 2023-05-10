using Microsoft.EntityFrameworkCore;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace WebApiVK.Domain;

public class UsersRepository : IUsersRepository
{
    private readonly UsersContext context;
    private readonly IDateTimeRecorder recorder;

    public UsersRepository(IDateTimeRecorder recorder, UsersContext context)
    {
        this.recorder = recorder;
        this.context = context;
    }

    public async Task<UserEntity> FindByLogin(string login)
    {
        var user = await context.Users
            .Include(x => x.Group)
            .Include(x => x.State)
            .FirstOrDefaultAsync(x => x.Login == login);
        
        if (user == null) return null;
        
        return Clone(user);
    }

    public async Task<PageList<UserEntity>> GetPage(int pageNumber, int pageSize)
    {
        var count = context.Users.Count();
        var orderedItems = await context.Users
            .Include(x => x.Group)
            .Include(x => x.State)
            .OrderBy(u => u.Login)
            .ToListAsync();

        var itemsOnPage = orderedItems.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var entities = itemsOnPage.Select(u => Clone(u)).ToList();

        return new PageList<UserEntity>(entities, count, pageNumber, pageSize);
    }

    public async Task<UserEntity> Insert(UserEntity user)
    {
        var userToAdd = Clone(user);

        if (userToAdd.Group == null)
        {
            userToAdd.Group = await context.UserGroups
                .FirstAsync(x => x.Code == GroupType.User);
        }

        if (userToAdd.State == null)
        {
            userToAdd.State = await context.UserStates
                .FirstAsync(x => x.Code == StateType.Active);
        }

        userToAdd.Created = recorder.GetCurrentDateTime();

        var entry = await context.AddAsync(userToAdd);
        var addedUser = Clone(entry.Entity);

        await context.SaveChangesAsync();
        return addedUser;
    }

    public async Task<UserEntity> BlockUserByLogin(string login)
    {
        var user = await context.Users
            .Include(x => x.Group)
            .Include(x => x.State)
            .FirstOrDefaultAsync(x => x.Login == login);

        if (user == null) return null;

        var blockState = await FindUserState(StateType.Blocked);

        if (blockState == null)
            throw new ArgumentNullException();

        user.State = blockState;
        await context.SaveChangesAsync();

        return Clone(user);
    }

    private async Task<UserState> FindUserState(StateType type)
    {
        var state = await context.UserStates
            .FirstAsync(x => x.Code == type);

        return state;
    }

    private UserEntity Clone(UserEntity user)
    {
        return new UserEntity(user.Id, user.Login, user.Created, user.Group, user.State, user.Password);
    }
}