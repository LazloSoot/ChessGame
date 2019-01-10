using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;
using Chess.Common.Interfaces;
using System.Collections.Generic;
using Chess.BusinessLogic.Interfaces.SignalR;

namespace Chess.BusinessLogic.Services
{
    public class ChessMovesService : CRUDService<Move, MoveDTO> , IChessMovesService
    {
        //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        // 0-позиция фигур                             1 2    3 4 5
        // 0 - позиция фигур,  1 - чей ход, 2 - флаги рокировки
        // 3 - правило битого поля, 4 - колич. ходов для правила 50 ходов
        // 5 - номер хода
        private readonly ICurrentUser _currentUserProvider;
        private readonly IUserService _userService;
        private readonly IChessGame _chessGame;
        private readonly ISignalRChessService _signalRChessService;

        public ChessMovesService(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            ICurrentUser currentUserProvider,
            IUserService userService,
            IChessGame chessGame,
            ISignalRChessService signalRChessService
            )
            : base(mapper, unitOfWork)
        {
            _currentUserProvider = currentUserProvider;
            _userService = userService;
            _chessGame = chessGame;
            _signalRChessService = signalRChessService;
        }

        public async Task<MoveDTO> Move(MoveRequest moveRequest)
        {
#warning кинуть разные исключения вместо null
            if (uow == null || string.IsNullOrWhiteSpace(moveRequest.Move) || moveRequest.GameId < 1)
                return null;

            var gameDbRecord = await uow.GetRepository<Game>().GetByIdAsync(moveRequest.GameId);
            if (gameDbRecord == null || gameDbRecord.Status != DataAccess.Helpers.GameStatus.Going)
                return null;

            var currentUser = await _userService.GetByUid(_currentUserProvider.GetCurrentUserUid());
            if (gameDbRecord.Sides.Where(s => s.PlayerId == currentUser.Id).FirstOrDefault() == null)
                return null;

            var game = _chessGame.InitGame(gameDbRecord.Fen);
            var gameAfterMove = game.Move(moveRequest.Move);
            if (game.Equals(gameAfterMove))
                return null;

            gameDbRecord.Fen = gameAfterMove.Fen;
            var move = new Move()
            {
                MoveNext = moveRequest.Move,
                Fen = game.Fen,
                Player = mapper.Map<User>(currentUser),
                Ply = (gameDbRecord.Moves.Count() + 1) / 2
            };


            gameDbRecord.Moves.Add(move);
            await uow.SaveAsync();
            var committedMove = mapper.Map<MoveDTO>(move);
            committedMove.MoveNext = moveRequest.Move;
            committedMove.FenAfterMove = gameAfterMove.Fen;
            await _signalRChessService.CommitMove(gameDbRecord.Id);
            return committedMove;
        }

        public async Task Resign()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetAllValidMovesForFigureAt(int gameId, string squareName)
        {
            if (uow == null)
                return null;

            if (squareName.Length != 2)
                return null;

            int x, y;
            if (squareName[0] >= 'a' &&
                squareName[0] <= 'h' &&
                squareName[1] >= '1' &&
                squareName[1] <= '8')
            {
                x = squareName[0] - 'a';
                y = squareName[1] - '1';
            } else
            {
                return null;
            }

            var targetGame = await uow.GetRepository<Game>().GetByIdAsync(gameId);
#warning кинуть исключение (игры нет)
            if (targetGame == null)
                return null;

            var chessGame = _chessGame.InitGame(targetGame.Fen);
            return chessGame.GetAllValidMovesForFigureAt(x,y);
        }
    }
}
