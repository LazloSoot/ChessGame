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
        private readonly IChessMovesService _chessMovesService;

        public GamesController(IGameDataService service, IChessMovesService chessMovesService)
        {
            _service = service;
            _chessMovesService = chessMovesService;
        }

        ////GET: Games
        //[HttpGet()]
        //public async Task<IActionResult> GetMyGames()
        //{
        //    var games = await _service.GetListAsync();
        //    return games == null ? NotFound("No games found!") as IActionResult
        //        : Ok(games);
        //}

        // GET: Games/5
        [HttpGet("{id}", Name = "GetGame")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _service.GetByIdAsync(id);
            return game == null ? NotFound($"Game with id = {id} not found!") as IActionResult
                : Ok(game);
        }

        // GET: Games/{gameId}/moves/available/{squareName}
        [HttpGet("{gameId}/moves/available/{squareName}")]
        public async Task<IActionResult> GetAllValidMovesForFigureAt(int gameId, string squareName)
        {
            var moves = await _chessMovesService.GetAllValidMovesForFigureAt(gameId, squareName);
            return moves == null ? NotFound($"No moves available for figure at {squareName}") as IActionResult
                : Ok(moves);
        }

        // POST: Games
        [HttpPost(Name ="CreateGameWithFriend")]
        public async Task<IActionResult> CreateGameWithFriend([FromBody]GameFullDTO game)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await _service.CreateNewGameWithFriend(game);
            return entity == null ? StatusCode(409) as IActionResult
                : Ok(entity);
        }

        // POST: Games/AI
        [HttpPost("ai", Name = "CreateGameVersusAI")]
        public async Task<IActionResult> CreateGameVersusAI([FromBody]GameFullDTO game)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await _service.CreateNewGameVersusAI(game);
            return entity == null ? StatusCode(409) as IActionResult
                : Ok(entity);
        }

        // POST: Games/player
        [HttpPost("player", Name = "CreateGameVersusRandPlayer")]
        public async Task<IActionResult> CreateGameVersusRandPlayer([FromBody]GameFullDTO game)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await _service.CreateNewGameWithFriend(game);
            return entity == null ? StatusCode(409) as IActionResult
                : Ok(entity);
        }

        // PUT: Games/{:id}/join
        [HttpPut("{gameId}/join", Name = "JoinGame")]
        public async Task<IActionResult> JoinGame(int gameId)
        {
            var readyGame = await _service.JoinToGame(gameId);
            return readyGame == null ? StatusCode(304) as IActionResult
                : Ok(readyGame);
        }
    }
}
