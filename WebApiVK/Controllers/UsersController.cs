﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WebApiVK.Attributes;
using WebApiVK.Authorization;
using WebApiVK.Domain;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace WebApiVK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUsersRepository repository;
        private readonly IMapper mapper;
        private readonly IEncryptor encryptor;
        private readonly ICoder coder;


        public UsersController(ILogger<UsersController> logger, 
            IUsersRepository repository, 
            IMapper mapper,
            IEncryptor encryptor,
            ICoder coder)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.encryptor = encryptor;
            this.coder = coder;
        }
        
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        [Authorize(Roles = "admin")]
        public ActionResult<UserEntity> GetUserById([FromRoute] Guid userId)
        {
            var user = repository.FindById(userId);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult<UserEntity> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 1 : pageSize;
            pageSize = pageSize > 20 ? 20 : pageSize;
            
            return Ok();
        }

        // Допускаем к созданию либо админа, либо неаутентифицированного пользователя
        [HttpPost]
        [AllowAnonymousOrAdmin]
        public IActionResult CreateUser([FromBody] UserToCreateDto user)
        {
            // Валидация происходит с помощью FluentValidation
            var login = coder.Decode(user.Login);
            var password = coder.Decode(user.Password);

            var encrypted = encryptor.EncryptPassword(password);
            var newUser = new UserToCreateDto(login, encrypted);

            var userEntity = mapper.Map<UserEntity>(newUser);
            var addedUser = repository.Insert(userEntity);

            var response = CreatedAtRoute(nameof(GetUserById), new { login = addedUser.Login }, addedUser.Login);
            return response;
        }
    }
}
