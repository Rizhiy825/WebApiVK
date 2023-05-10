namespace WebApiVK.Models;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public DateTime Created { get; set; }
    public int UserGroupId { get; set; }
    public UserGroup Group { get; set; }
    public int UserStateId { get; set; }
    public UserState State { get; set; }

    public UserEntity() { }

    public UserEntity(string login, string password, UserGroup group, UserState state)
    {
        Login = login;
        Group = group;
        Password = password;
        State = state;
    }

    public UserEntity(Guid id, string login, DateTime created, UserGroup group, UserState state, string password)
    {
        Id = id;
        Login = login;
        Created = created;

        if (group != null) UserGroupId = group.Id;
        Group = group;

        if (state != null) UserStateId = state.Id;
        State = state;

        Password = password;
    }
}