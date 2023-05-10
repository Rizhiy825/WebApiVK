using Microsoft.EntityFrameworkCore;
using WebApiVK.Interfaces;

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

    public async Task<UserEntity> FindById(Guid id)
    {
        var user = await context.Users.FindAsync(id);

        if (user == null) return null;

        return user;
    }
    
    public async Task<UserEntity> FindByLogin(string login)
    {
        var user = await context.Users
            .Include(x => x.Group)
            .Include(x => x.State)
            .FirstOrDefaultAsync(x => x.Login == login);
        
        if (user == null) return null;

        return user;
    }

    public async Task<UserEntity> Insert(UserEntity user)
    {
        if (user.Group == null)
        {
            user.Group = await context.UserGroups
                .FirstAsync(x => x.Code == GroupType.User);
        }

        if (user.State == null)
        {
            user.State = await context.UserStates
                .FirstAsync(x => x.Code == StateType.Active);
        }
        
        user.Created = recorder.GetCurrentDateTime();

        var entry = await context.AddAsync(user);
        var addedUser = entry.Entity;

        await context.SaveChangesAsync();
        return addedUser;
    }

    public async Task<UserEntity> BlockUser(string login)
    {
        var userForBlocking = await FindByLogin(login);

        if (userForBlocking == null) return null;

        var blockState = await FindUserState(StateType.Blocked);

        if (blockState == null)
            throw new ArgumentNullException();

        userForBlocking.State = blockState;
        return userForBlocking;
    }

    private async Task<UserState> FindUserState(StateType type)
    {
        var state = await context.UserStates
            .FirstAsync(x => x.Code == type);

        return state;
    }

    //public async Task<Login> AddLoginToQueue(string login)
    //{
    //    var newLogin = new Login(login);
    //    var addedLogin = await context.LoginQueue.AddAsync(newLogin);
    //    await context.SaveChangesAsync();
    //    return addedLogin.Entity;
    //}

    //public async Task<bool> IsLoginInQueue(string login)
    //{
    //    var found = await context.LoginQueue.FindAsync(new string [] {login});

    //    return found != null;
    //}
}