using Microsoft.EntityFrameworkCore;
using WebApiVK.Domain;

namespace WebApiVK.Interfaces;

public interface IContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserState> UserStates { get; set; }
}