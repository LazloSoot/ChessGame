using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chess.BusinessLogic.Interfaces;
using Chess.DataAccess.Entities;
using Chess.Common.DTOs;

namespace ChessWeb.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ICRUDService<Game, GameDTO> service;

        public GamesController(ICRUDService<Game, GameDTO> service)
        {
            this.service = service;
        }

        // GET: Games
        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await service.GetListAsync();
            return games == null ? NotFound("No games found!") as IActionResult
                : Ok(games);
        }

        // GET: Games/5
        [HttpGet("{id}", Name = "GetGame")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await service.GetByIdAsync(id);
            return game == null ? NotFound($"Game with id = {id} not found!") as IActionResult
                : Ok(game);
        }

        // POST: Games
        public async Task<IActionResult> AddGame([FromBody]GameDTO game)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.AddAsync(game);
            return entity == null ? StatusCode(409) as IActionResult
                : StatusCode(201) as IActionResult;
        }

        // DELETE: Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var success = await service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
