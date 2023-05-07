using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WebApiVK.Domain;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public DateTime Created { get; set; }

    //public int UserGroupId { get; set; }
    public UserGroup Group { get; set; }

    //public int UserStateId { get; set; }
    public UserState State { get; set; }

    public UserEntity()
    {
        Id = Guid.NewGuid();
    }

    public UserEntity(Guid id)
    {
        Id = id;
    }
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
        Group = group;
        State = state;
        Password = password;
    }
}