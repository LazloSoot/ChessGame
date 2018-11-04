using System.Threading.Tasks;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ChessWeb.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ICRUDService<Player, PlayerDTO> service;

        public PlayersController(ICRUDService<Player, PlayerDTO> service)
        {
            this.service = service;
        }

        // GET: Players
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await service.GetListAsync();
            return players == null ? NotFound("No players found!") as IActionResult
                : Ok(players);
        }

        // GET: Players/5
        [HttpGet("{id}", Name = "GetPlayer")]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var player = await service.GetByIdAsync(id);
            return player == null ? NotFound($"Player with id = {id} not found!") as IActionResult
                : Ok(player);
        }

        // POST: Players
        //public async Task<IActionResult> AddPlayer([FromBody]PlayerDTO player)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest() as IActionResult;

        //    var entity = await service.AddAsync(player);
        //    return entity == null ? StatusCode(409) as IActionResult
        //        : StatusCode(201) as IActionResult;
        //}

        // DELETE: Players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var success = await service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
