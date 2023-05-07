using Microsoft.EntityFrameworkCore;
using System;
using WebApiVK.Interfaces;

namespace WebApiVK.Domain;

public class UsersContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserGroup> UserGroups { get; set; } = null!;
    public DbSet<UserState> UserStates { get; set; } = null!;

    private IEncryptor encryptor;

    public UsersContext(DbContextOptions<UsersContext> options, IEncryptor encryptor)
        : base(options)
    {
        this.encryptor = encryptor;
        //Database.EnsureDeleted();
        Database.EnsureCreated(); // гарантируем, что БД создана

        if (!UserGroups.Any())
        {
            UserGroups.AddRange(
                new UserGroup(GroupType.Admin, "Full access"),
                new UserGroup(GroupType.User, "Limited access"));
            SaveChanges();
        }

        if (!UserStates.Any())
        {
            UserStates.AddRange(
                new UserState(StateType.Active, "Account is active"),
                new UserState(StateType.Blocked, "Account was disabled"));
            SaveChanges();
        }

        if (!Users.Any())
        {
            var encryptedPassword = this.encryptor.EncryptPassword("admin");
            var adminGroup = UserGroups.Where(x => x.Code == GroupType.Admin).First();
            var adminState = UserStates.Where(x => x.Code == StateType.Active).First();
            var admin = new UserEntity("admin", encryptedPassword, adminGroup, adminState);
            Users.Add(admin);

            SaveChanges();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().HasAlternateKey(u => u.Login);
    }
}