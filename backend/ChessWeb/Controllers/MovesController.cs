using System.Threading.Tasks;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessWeb.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class MovesController : ControllerBase
    {
        private readonly IChessMovesService service;

        public MovesController(IChessMovesService service)
        {
            this.service = service;
        }

        // GET: moves/{:moveId}
        [HttpGet("{moveId}", Name = "GetMove")]
        public async Task<IActionResult> GetMove(int moveId)
        {
            var move = await service.GetByIdAsync(moveId);
            return move == null ? NotFound($"Move with moveId = {moveId} not found!") as IActionResult
                : Ok(move);
        }

        // POST: moves
        [HttpPost]
        public async Task<IActionResult> Move([FromBody]MoveRequest move)
        {
            var _move = await service.Move(move);
            return _move == null ? StatusCode(409) as IActionResult
                : Ok(_move) as IActionResult;
        }

        // DELETE: Moves/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMove(int gameId)
        {
            var success = await service.TryRemoveAsync(gameId);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
