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
        private readonly IGameDataService _gameDataService;

        public UsersController(IUserService service, 
            IGameDataService gameDataService
            )
        {
            _service = service;
            _gameDataService = gameDataService;
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

        // GET: Users/
        [HttpGet(Name= "SearchUsers")]
        public async Task<IActionResult> SearchUsers([FromQuery(Name ="part")]string part, [FromQuery(Name = "isOnline")]bool isOnline, [FromQuery(Name = "pageIndex")]int? pageIndex, [FromQuery(Name = "pageSize")]int? pageSize)
        {
            var users = await _service.SearchUsers(part, isOnline, pageIndex, pageSize);
            return users == null ? NotFound($"No users found!") as IActionResult
                : Ok(users);
        }

        // GET: users/{userId}/games
        [HttpGet("{userId}/games", Name = "GetUserGames")]
        public async Task<IActionResult> GetUserGames(int userId, [FromQuery(Name = "pageIndex")] int? pageIndex, [FromQuery(Name = "pageSize")] int? pageSize)
        {
            var gamesPage = await _gameDataService.GetUserGames(userId, pageIndex, pageSize);
            return gamesPage == null ? NotFound("No games found!") as IActionResult
                : Ok(gamesPage);
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

        [Route("/search/reindex")]
        public async Task<IActionResult> ReIndex()
        {
            var result = await _service.ReIndex();
            return Ok(result);
        }
    }
}
