﻿using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Chess.BL;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Chess.BusinessLogic.Services
{
    public class ChessMovesService : CRUDService<Move, CommitedMoveDTO> , IChessMovesService
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

        public async Task<CommitedMoveDTO> Move(MoveDTO moveRequest)
        {
#warning кинуть разные исключения вместо null
            if (uow == null || string.IsNullOrWhiteSpace(moveRequest.Move) || moveRequest.GameId < 1)
                return null;

            return await AddAsync(new CommitedMoveDTO()
            {
                GameId = moveRequest.GameId,
                MoveNext = moveRequest.Move
            });
        }

        public async Task<CommitedMoveDTO> CommitMove(CommitedMoveDTO move)
        {
            var game = await FindGame(move);

            if (game == null || game.Status != DataAccess.Helpers.GameStatus.Going)
                return null;

            await FormMoveEntry();
#warning оповестить signalr
            var fenAfterMove = CommitMove(move.FenBeforeMove, move.MoveNext);
            if (!string.IsNullOrEmpty(fenAfterMove))
            {
                var committedMove = await base.AddAsync(move);
                committedMove.FenAfterMove = fenAfterMove;
                return committedMove;
            }
            else
                return null;
            
            async Task FormMoveEntry()
            {
#warning получить информацию о current user
                
                var gameMoves = await uow.GetRepository<Move>()
                    .GetAllAsync(m => m.GameId == move.GameId);

                if(gameMoves?.Count() < 1)
                {
                    move.FenBeforeMove = game.Fen;
                    move.Ply = 1;
                }
                else
                {
                    var prevMove = gameMoves.OrderByDescending(m => m.Ply).First();
                    var currentFen = CommitMove(prevMove.Fen, prevMove.MoveNext);
                    if (string.IsNullOrWhiteSpace(currentFen))
                        throw new ArgumentNullException($"Fen of move with id ={prevMove.Id} is corrupted!");

                    move.FenBeforeMove = currentFen;
                    move.Ply = prevMove.Ply + 1;
                }
            }
        }

        public async Task Resign()
        {
            throw new NotImplementedException();
        }

        private async Task<Game> FindGame(CommitedMoveDTO move)
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

        private string CommitMove(string currentFen, string move)
        {
#warning переработать логику Chess core
            var chess = new ChessGame(currentFen);
            var nextFen = chess.Move(move).Fen;
            if (string.Equals(chess.Fen, nextFen))
                return string.Empty;
            else
                return nextFen;
        }

    }
}
