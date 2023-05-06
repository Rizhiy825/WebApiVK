using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WebApiVK.Authorization;
using WebApiVK.Models;

namespace WebApiVK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }
        
        [Authorize]
        [HttpHead("{userId}")]
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        public ActionResult<User> GetUserById([FromRoute] Guid userId)
        {
            return Ok();
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<User> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 1 : pageSize;
            pageSize = pageSize > 20 ? 20 : pageSize;
            
            return Ok();
        }
    }
}
