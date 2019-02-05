using System.Threading.Tasks;
using Chess.BusinessLogic.Interfaces;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.DTOs;
using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISignalRNotificationService _notificationService;

        public UsersController(IUserService service, ISignalRNotificationService notificationService)
        {
            _service = service;
            _notificationService = notificationService;
        }

        // GET: Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetListAsync();
            return users == null ? NotFound("No users found!") as IActionResult
                : Ok(users);
        }

        // GET: Users/5
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return user == null ? NotFound($"User with id = {id} not found!") as IActionResult
                : Ok(user);
        }

        // GET:Users/current
        [HttpGet("current", Name = "GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _service.GetCurrentUser();
            return user == null ? StatusCode(500) as IActionResult
                : Ok(user);
        }

        // GET: Users/online
        [HttpGet("online", Name ="GetOnlineUsers")]
        public ActionResult GetOnlineUsers()
        {
            return Ok(_notificationService.GetOnlineUsersInfo());
        }

        // GET: Users/online/:{part}
        [HttpGet("online/{part}", Name= "GetOnlineUsersInfoByNameStartsWith")]
        public ActionResult GetOnlineUsersInfoByNameStartsWith(string part)
        {
            return Ok(_notificationService.GetOnlineUsersInfoByNameStartsWith(part));
        }
        // POST: Users
        public async Task<IActionResult> AddUser([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await _service.AddAsync(user);
            return entity == null ? StatusCode(409) as IActionResult
                : Ok(entity) as IActionResult;
        }

        // DELETE: Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
