﻿namespace WebApiVK.Domain;

public class NpgsqlUsersRepository : IUsersRepository
{
    private readonly UsersContext _context;

    public NpgsqlUsersRepository(UsersContext context)
    {
        _context = context;
    }

    public UserEntity GetUserById(Guid id)
    {
        var user = _context.Users.Find(id);
        return user;
    }
}