using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Chess.BL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Services
{
    public class ChessMovesService : CRUDService<Move, MoveDTO> , IChessMovesService
    {
        public ChessMovesService(IMapper mapper, IUnitOfWork unitOfWork)
            : base(mapper, unitOfWork)
        {

        }

        public override async Task<MoveDTO> AddAsync(MoveDTO move)
        {
            if (uow == null || string.IsNullOrWhiteSpace(move.MoveNext))
                return null;

            var actualFen = await GetCurrentGameFEN(move);
            if (string.IsNullOrEmpty(actualFen))
                return null;

            move.Fen = actualFen;
            var fenAfterMove = CommitMove(move);
            if (!string.IsNullOrEmpty(fenAfterMove))
            {
                await FormMoveEntry(move);
#warning обновить Game fen
                return await base.AddAsync(move);
            }
            else
                return null;
        }

        private async Task<string> GetCurrentGameFEN(MoveDTO move)
        {
            int gameId;

            if (uow == null)
                return string.Empty;

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
                return string.Empty;
            }

            var game = await uow.GetRepository<Game>().GetByIdAsync(gameId);
            if (game?.Status == DataAccess.Helpers.GameStatus.Going)
                return game.Fen;

            return string.Empty;
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
        }
    }
}
