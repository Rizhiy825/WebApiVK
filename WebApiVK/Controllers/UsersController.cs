using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WebApiVK.Authorization;
using WebApiVK.Domain;

namespace WebApiVK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersRepository _repository;

        public UsersController(ILogger<UsersController> logger, IUsersRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        
        [Authorize]
        [HttpHead("{userId}")]
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        public ActionResult<UserEntity> GetUserById([FromRoute] Guid userId)
        {
            var user = _repository.GetUserById(userId);
            return Ok();
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<UserEntity> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 1 : pageSize;
            pageSize = pageSize > 20 ? 20 : pageSize;
            
            return Ok();
        }
    }
}
