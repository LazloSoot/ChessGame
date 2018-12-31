using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chess.BusinessLogic.Interfaces;
using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ChessWeb.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IGameDataService _service;

        public GamesController(IGameDataService service)
        {
            _service = service;
        }

        // GET: Games
        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _service.GetListAsync();
            return games == null ? NotFound("No games found!") as IActionResult
                : Ok(games);
        }

        // GET: Games/5
        [HttpGet("{id}", Name = "GetGame")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _service.GetByIdAsync(id);
            return game == null ? NotFound($"Game with id = {id} not found!") as IActionResult
                : Ok(game);
        }

        // POST: Games
        public async Task<IActionResult> CreateNewGame([FromBody]GameDTO game)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await _service.CreateNewGame(game);
            return entity == null ? StatusCode(409) as IActionResult
                : Ok(entity);
        }

        // PUT: Games
        [HttpPut]
        public async Task<IActionResult> JoinGame([FromBody]SideDTO side)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var readyGame = await _service.JoinToGame(side);
            return readyGame as IActionResult;
        }

        // DELETE: Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var success = await _service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
