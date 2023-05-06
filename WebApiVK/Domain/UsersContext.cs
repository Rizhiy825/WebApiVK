using Microsoft.EntityFrameworkCore;
using WebApiVK.Models;

namespace WebApiVK.Domain;

public class UsersContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public UsersContext(DbContextOptions<UsersContext> options)
        : base(options)
    {
        Database.EnsureCreated(); // гарантируем, что БД создана
    }
}