using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly LinkGenerator linkGenerator;

        public UsersController(ILogger<UsersController> logger, 
            IUsersRepository repository, 
            IMapper mapper,
            IEncryptor encryptor,
            ICoder coder,
            ILoginsManager loginsManager,
            LinkGenerator linkGenerator)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.encryptor = encryptor;
            this.coder = coder;
            this.loginsManager = loginsManager;
            this.linkGenerator = linkGenerator;
        }
        
        [HttpGet("{login}", Name = nameof(GetUserByLogin))]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserEntity>> GetUserByLogin([FromRoute] string login)
        {
            var user = await repository.FindByLogin(login);

            if (user == null)
                return NotFound();

            // UserToReturnDto не включает в себя пароль. В целях безопастности его не возвращаем
            var userToReturn = mapper.Map<UserToReturnDto>(user);

            return Ok(userToReturn);
        }

        // Получать пользователей 
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserEntity>> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 1 : pageSize;
            pageSize = pageSize > 20 ? 20 : pageSize;

            var users = await repository.GetPage(pageNumber, pageSize);

            var usersToReturn = mapper.Map<IEnumerable<UserToReturnDto>>(users);

            string nextUri;
            string previousUri = null;

            if (pageNumber > 1)
                previousUri = linkGenerator.GetUriByRouteValues(HttpContext, null, new { pageNumber = pageNumber - 1, pageSize });

            nextUri = linkGenerator.GetUriByRouteValues(HttpContext, null, new { pageNumber = pageNumber + 1, pageSize });
            var pagination = new
            {
                previousPageLink = previousUri,
                nextPageLink = nextUri,
                totalCount = users.TotalCount,
                pageSize = pageSize,
                currentPage = pageNumber,
                totalPages = users.TotalPages,
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

            return Ok(usersToReturn);
        }
        
        // Допускаем к созданию либо админа, либо неаутентифицированного пользователя
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateUser([FromBody] UserToCreateDto user)
        {
            var authUser = User.IsInRole("User");

            if (authUser)
                return Forbid();

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

            var addedUser = new UserEntity();

            // Тут уже не переживаем, что в репо добавляется пользователь с таким же логином
            try
            {
                addedUser = await repository.Insert(userEntity);
            }
            catch (DbUpdateException)
            {
                loginsManager.TryRemoveLogin(login);
            }
            
            await Task.Delay(new TimeSpan(0, 0, 0, 5));

            // Проверку выполнять необязательно, на этом этапе не может быть два потока,
            // которые одновременно пытаются удалить один логин из очереди. 
            loginsManager.TryRemoveLogin(login);
            
            var userToReturn = mapper.Map<UserToReturnDto>(addedUser);
            var response = CreatedAtRoute(nameof(GetUserByLogin), new { login = login }, userToReturn);
            return response;
        }

        // Решено не отправлять в ответ данные пользователя (в соответствии с рекомендациями ответов http).
        // Если админ захочет проверить статус - сделает запрос GET
        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string login)
        {
            if (login.Length < 4 || login.Length > 30)
            {
                return NotFound();
            }

            var repoUser = await repository.BlockUserByLogin(login);

            if (repoUser == null) return NotFound();
            
            return NoContent();
        }
    }
}
