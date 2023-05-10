using WebApiVK.Models;

namespace WebApiVK.Authorization;

public class UserToAuth
{
    public Guid Id { get; }
    public string Login { get; }
    public UserGroup Group { get; }
    public UserState State { get; }
    public UserToAuth() { }

    public UserToAuth(Guid id, string login, UserGroup group, UserState state)
    {
        Id = id;
        Login = login;
        Group = group;
        State = state;
    }
}