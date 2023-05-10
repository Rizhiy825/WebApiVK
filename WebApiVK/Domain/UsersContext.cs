using Microsoft.EntityFrameworkCore;
using System;
using WebApiVK.Interfaces;
using WebApiVK.Models;

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
        
        Database.EnsureCreated(); // гарантируем, что БД создана

        // Заполняем начальными данными в случае, если БД пустая
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
            var adminGroup = UserGroups.First(x => x.Code == GroupType.Admin);
            var activeState = UserStates.First(x => x.Code == StateType.Active);
            var admin = new UserEntity("admin", encryptedPassword, adminGroup, activeState);
            Users.Add(admin);

            encryptedPassword = this.encryptor.EncryptPassword("qwerty");
            var userGroup = UserGroups.First(x => x.Code == GroupType.User);
            var user = new UserEntity("user", encryptedPassword, userGroup, activeState);
            Users.Add(user);

            SaveChanges();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Как выяснилось, InMemoryDataBase не поддерживает реализацию уникальных ключей
        modelBuilder.Entity<UserEntity>().HasAlternateKey(u => u.Login);
    }
}