﻿using WebApiVK.Domain;
using WebApiVK.Models;

namespace WebApiVK.Interfaces;

public interface IUsersRepository
{
    public Task<UserEntity> FindByLogin(string login);
    public Task<UserEntity> Insert(UserEntity userEntity);
    public Task<UserEntity> BlockUserByLogin(string login);
    public Task<PageList<UserEntity>> GetPage(int pageNumber, int pageSize);
}