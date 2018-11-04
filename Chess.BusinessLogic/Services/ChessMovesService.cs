using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Chess.BL;
using System;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Services
{
    public class ChessMovesService : CRUDService<Move, MoveDTO> , IChessMovesService
    {
        //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        // 0-позиция фигур                             1 2    3 4 5
        // 0 - позиция фигур,  1 - чей ход, 2 - флаги рокировки
        // 3 - правило битого поля, 4 - колич. ходов для правила 50 ходов
        // 5 - номер хода

        public ChessMovesService(IMapper mapper, IUnitOfWork unitOfWork)
            : base(mapper, unitOfWork)
        {

        }

        public override async Task<MoveDTO> AddAsync(MoveDTO move)
        {
            if (uow == null || string.IsNullOrWhiteSpace(move.MoveNext))
                return null;

            var game = await FindGame(move);
#warning кинуть разные исключения вместо null
            if (game == null 
                || string.IsNullOrEmpty(game.Fen) 
                || game.Status != DataAccess.Helpers.GameStatus.Going
                )
            {
                return null;
            }

            move.Fen = game.Fen;
            var fenAfterMove = CommitMove(move);
            if (!string.IsNullOrEmpty(fenAfterMove))
            {
                await FormMoveEntry(move);
                game.Fen = move.Fen;
                uow.GetRepository<Game>().Update(game);
                return await base.AddAsync(move);
            }
            else
                return null;
        }

        private async Task<Game> FindGame(MoveDTO move)
        {
            int gameId;

            if (uow == null)
                return null;

            if (move.Game != null)
            {
                gameId = move.Game.Id;
            }
            else if (move.GameId.HasValue)
            {
                gameId = move.GameId.Value;
            }
            else
            {
                return null;
            }

            return await uow.GetRepository<Game>().GetByIdAsync(gameId);
        }

        private string CommitMove(MoveDTO move)
        {
#warning переработать логику Chess core
            var chess = new ChessGame(move.Fen);
            var nextFen = chess.Move(move.MoveNext).Fen;
            if (string.Equals(chess.Fen, nextFen))
                return string.Empty;
            else
                return nextFen;
        }

        private async Task FormMoveEntry(MoveDTO move)
        {
#warning получить информацию о current user
            // инициализировать Ply

            var fen = move.Fen;
            int.TryParse(fen.Substring(fen.Length - 2, 1), out int currentMoveIndex);
            int ply = currentMoveIndex / 2;
            if(currentMoveIndex < 0)
            {
                throw new ArgumentException("Fen is not valid, there is no move index given!");
            }
            else
            {
                var prevMove = await uow.GetRepository<Move>()
                    .GetOneAsync(m => m.GameId == move.GameId && m.Ply == ply - 1);
                if (prevMove == null)
                    move.Ply = ply - 1;
                else
                    move.Ply = ply;
            }
        }
    }
}
