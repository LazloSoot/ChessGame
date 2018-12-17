using System.Threading.Tasks;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
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
        private readonly IUserService service;

        public UsersController(IUserService service)
        {
            this.service = service;
        }

        // GET: Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await service.GetListAsync();
            return users == null ? NotFound("No users found!") as IActionResult
                : Ok(users);
        }

        // GET: Users/5
        [HttpGet("{uid}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string uid)
        {
            var user = await service.GetByUid(uid);
            return user == null ? NotFound($"User with id = {uid} not found!") as IActionResult
                : Ok(user);
        }

        // POST: Users
        public async Task<IActionResult> AddUser([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.AddAsync(user);
            return entity == null ? StatusCode(409) as IActionResult
                : StatusCode(201) as IActionResult;
        }

        // DELETE: Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await service.TryRemoveAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
