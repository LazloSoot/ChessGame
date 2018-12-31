using System.Threading.Tasks;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ChessWeb.Controllers
{
    [Route("games/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class SidesController : ControllerBase
    {
        private readonly ICRUDService<Side, SideDTO> service;

        public SidesController(ICRUDService<Side, SideDTO> service)
        {
            this.service = service;
        }

        // GET: Games/Sides
        [HttpGet]
        public async Task<IActionResult> GetAllSides()
        {
            var sides = await service.GetListAsync();
            return sides == null ? NotFound("No sides found!") as IActionResult
                : Ok(sides);
        }

        // GET: Games/Sides/5
        [HttpGet("{id}", Name = "GetSide")]
        public async Task<IActionResult> GetSide(int id)
        {
            var side = await service.GetByIdAsync(id);
            return side == null ? NotFound($"Side with id = {id} not found!") as IActionResult
                : Ok(side);
        }

        
    }
}
