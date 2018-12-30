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
    public class MovesController : ControllerBase
    {
        private readonly IChessMovesService service;

        public MovesController(IChessMovesService service)
        {
            this.service = service;
        }

        // GET: Moves
        [HttpGet]
        public async Task<IActionResult> GetAllMoves()
        {
            var moves = await service.GetListAsync();
            return moves == null ? NotFound("No moves found!") as IActionResult
                : Ok(moves);
        }

        // GET: Moves/5
        [HttpGet("{id}", Name = "GetMove")]
        public async Task<IActionResult> GetMove(int id)
        {
            var move = await service.GetByIdAsync(id);
            return move == null ? NotFound($"Move with id = {id} not found!") as IActionResult
                : Ok(move);
        }

        // POST: Moves
        public async Task<IActionResult> Move([FromBody]MoveRequest move)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.Move(move);
            return entity == null ? StatusCode(409) as IActionResult
                : StatusCode(201) as IActionResult;
        }

        // DELETE: Moves/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMove(int id)
        {
            var success = await service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
