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
    public class SidesController : ControllerBase
    {
        private readonly ICRUDService<Side, SideDTO> service;

        public SidesController(ICRUDService<Side, SideDTO> service)
        {
            this.service = service;
        }

        // GET: Sides
        [HttpGet]
        public async Task<IActionResult> GetAllSides()
        {
            var sides = await service.GetListAsync();
            return sides == null ? NotFound("No sides found!") as IActionResult
                : Ok(sides);
        }

        // GET: Sides/5
        [HttpGet("{id}", Name = "GetSide")]
        public async Task<IActionResult> GetSide(int id)
        {
            var side = await service.GetByIdAsync(id);
            return side == null ? NotFound($"Side with id = {id} not found!") as IActionResult
                : Ok(side);
        }

        // POST: Sides
        public async Task<IActionResult> AddSide([FromBody]SideDTO side)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.AddAsync(side);
            return entity == null ? StatusCode(409) as IActionResult
                : StatusCode(201) as IActionResult;
        }

        // DELETE: Sides/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSide(int id)
        {
            var success = await service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
