using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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

        private readonly HashSet<string> loginsInQueue = new HashSet<string>();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly ILoginsManager loginsManager;

        public UsersController(ILogger<UsersController> logger, 
            IUsersRepository repository, 
            IMapper mapper,
            IEncryptor encryptor,
            ICoder coder,
            ILoginsManager loginsManager)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.encryptor = encryptor;
            this.coder = coder;
            this.loginsManager = loginsManager;
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
        [AllowAnonymous]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateUser([FromBody] UserToCreateDto user)
        {
            // Валидация происходит с помощью FluentValidation
            var login = coder.Decode(user.Login);

            // Два потока могут одновременно зайти в IsLoginInQueue и пройти проверку,
            // но TryAddLoginToQueue точно отсеит один из потоков
            if (loginsManager.IsLoginInQueue(login) ||
                !loginsManager.TryAddLoginToQueue(login))
            {
                return Conflict();
            }

            var password = coder.Decode(user.Password);
            var encrypted = encryptor.EncryptPassword(password);

            var newUser = new UserToCreateDto(login, encrypted);
            var userEntity = mapper.Map<UserEntity>(newUser);
            
            // Тут уже не переживаем, что в репо добавляется пользователь с таким же логином
            var addedUser = await repository.Insert(userEntity);
            
            await Task.Delay(new TimeSpan(0, 0, 0, 5));

            // Проверку выполнять необязательно, на этом этапе не может быть два потока,
            // которые одновременно пытаются удалить один логин из очереди. 
            loginsManager.TryRemoveLogin(login);

            var response = CreatedAtRoute(nameof(GetUserById), new { userId = addedUser.Id }, addedUser.Id);
            return response;
        }
    }
}
