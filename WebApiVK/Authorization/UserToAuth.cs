using WebApiVK.Models;

namespace WebApiVK.Authorization;

public class UserToAuth
{
    public Guid Id { get; }
    public string Login { get; }
    public UserGroup Group { get; }

    public UserToAuth() { }

    public UserToAuth(Guid id, string login, UserGroup group)
    {
        Id = id;
        Login = login;
        Group = group;
    }
}