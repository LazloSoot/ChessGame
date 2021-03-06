﻿using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Chess.Common.Interfaces;
using Chess.Common.Helpers.ChessGame;
using Chess.BusinessLogic.Interfaces.SignalR;
using ChessGame.Core;

namespace Chess.BusinessLogic.Services
{
    public class GameDataService : CRUDService<Game, GameFullDTO>, IGameDataService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISignalRNotificationService _notificationService;
        private readonly ISignalRChessService _signalRChessService;
        public GameDataService(
            IMapper mapper, 
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider,
            ISignalRNotificationService notificationService,
            ISignalRChessService signalRChessService
            )
            : base(mapper, unitOfWork)
        {
            _notificationService = notificationService;
            _currentUserProvider = currentUserProvider;
            _signalRChessService = signalRChessService;
        }

#warning работает только с приглашением оппонента
        public async Task<GameFullDTO> CreateNewGameWithFriend(GameFullDTO game)
        {
            if (_uow == null)
                return null;

            if (string.IsNullOrWhiteSpace(game.Fen))
            {
                game.Fen = ChessGameEngine.DefaultFen;
            }
            else
            {
#warning кинуть исключение с информацией если Fen неккоректен
                // var game = new ChessGame(entity.Fen);
            }
            
            game.Status = DataAccess.Helpers.GameStatus.Waiting;
            var sides = game.Sides.ToList();
            var currentUserSide = sides.Where(s => s.Player == null).First();
            var opponent = sides.Where(s => s.Player != null).First().Player;
            var currentUser = await _currentUserProvider.GetCurrentDbUserAsync();
            currentUserSide.Player = mapper.Map<UserDTO>(currentUser);
            game.Sides = new List<SideDTO>()
            {
                currentUserSide
            };

            GameFullDTO targetGame;
            var gameRepo = _uow.GetRepository<Game>();
            var createdGame = await gameRepo
                                .GetOneAsync(g =>
                                g.Status == DataAccess.Helpers.GameStatus.Waiting && 
                                ((g.Sides.Count() == 1 && g.Sides.First().PlayerId == currentUser.Id)
                                ||
                                (g.Sides.Where(s => s.PlayerId == currentUser.Id).Count() > 0 && g.Sides.Where(s => s.Player == null).Count() > 0)));
            if(createdGame != null)
            {
                createdGame.Fen = game.Fen;
                createdGame.Sides = new List<Side>()
                {
                    mapper.Map<Side>(currentUserSide)
                };
                targetGame = mapper.Map<GameFullDTO>(await gameRepo.UpdateAsync(createdGame));
                await _uow.SaveAsync();
            }
            else
            {
                game.CreationDate = DateTime.Now;
                targetGame = await base.AddAsync(game);
            }
            await _notificationService.InviteUserAsync(opponent.Uid, targetGame.Id);
            return targetGame;
        }

        public async Task<GameFullDTO> CreateNewGameVersusAI(GameFullDTO game)
        {
            if (_uow == null)
                return null;

            if (string.IsNullOrWhiteSpace(game.Fen))
            {
                game.Fen = ChessGameEngine.DefaultFen;
            }

            var currentUser = await _currentUserProvider.GetCurrentUserAsync();
            var currentUserSide = game.Sides.Where(s => s.Player == null || s.PlayerId == currentUser.Id || s.Player.Id == currentUser.Id).First();
            currentUserSide.Player = mapper.Map<UserDTO>(currentUser);
            game.Sides = new List<SideDTO>()
            {
                currentUserSide
            };
            game.Status = DataAccess.Helpers.GameStatus.Going;

            game.CreationDate = DateTime.Now;
            return await base.AddAsync(game);
        }

        public async Task<GameFullDTO> JoinToGame(int gameId)
        {
            if (_uow == null)
                return null;

            var targetGame = await _uow.GetRepository<Game>().GetByIdAsync(gameId);
#warning кинуть исключение (игры нет\ нельзя подключится к игре)
            if (targetGame == null || targetGame.Status != DataAccess.Helpers.GameStatus.Waiting)
                return null;
            
            var hostSide = targetGame.Sides.FirstOrDefault();
#warning кинуть исключение, игра не валидна
            if (hostSide == null)
                return null;

            var color = (hostSide.Color == DataAccess.Helpers.Color.Black) ? DataAccess.Helpers.Color.White : DataAccess.Helpers.Color.Black;
            var currentDbUser = await _currentUserProvider.GetCurrentDbUserAsync();
            if (currentDbUser == null)
                return null;

            targetGame.Sides.Add(new Side()
            {
                Color = color,
                Player = currentDbUser
            });
            targetGame.Status = DataAccess.Helpers.GameStatus.Going;
            await _uow.GetRepository<Game>().UpdateAsync(targetGame);
            
            await _uow.SaveAsync();

            await _notificationService.AcceptInvitation(hostSide.Player.Uid, targetGame.Id);
            return mapper.Map<GameFullDTO>(targetGame);
        }

        public async Task<GameFullDTO> ResignGame(int gameId)
        {
            if (_uow == null)
                return null;

            var currentGame = await _uow.GetRepository<Game>().GetByIdAsync(gameId);

            if (currentGame == null)
                return null;

            var currentPlayer = await _currentUserProvider.GetCurrentDbUserAsync();
            var currentSide = currentGame.Sides.Where(s => s.PlayerId == currentPlayer.Id).FirstOrDefault();

            if (currentSide == null)
                return null;

            currentSide.IsResign = true;
            currentGame.Status = DataAccess.Helpers.GameStatus.Completed;
            await _uow.SaveAsync();
            await _signalRChessService.EmitResign(currentGame.Id, (Color)currentSide.Color);
            return mapper.Map<Game, GameFullDTO>(currentGame);
        }

        public async Task<GameFullDTO> SetDraw(int gameId)
        {
            return null;
        }

        public Task<GameFullDTO> SuspendGame(int gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDTO<GamePartialDTO>> GetUserGames(int userID, int? pageIndex, int? pageSize)
        {
            if (_uow == null)
                return null;

            var user = await _uow.GetRepository<User>().GetByIdAsync(userID);
            if (user == null)
                return null;

            var sidesPage = (await _uow.GetRepository<Side>()
                .GetAllPagedAsync(pageIndex, pageSize, s => s.PlayerId == user.Id));
            var gamesPage = new PagedResultDTO<GamePartialDTO>()
            {
                PageCount = sidesPage.PageCount,
                PageIndex = sidesPage.PageIndex,
                PageSize = sidesPage.PageSize,
                TotalDataRowsCount = sidesPage.TotalDataRowsCount,
                DataRows = mapper.Map<IEnumerable<GamePartialDTO>>(sidesPage.DataRows.Select(s => s.Game).ToList())
            };
            return gamesPage;
        } 

        public override async Task<PagedResultDTO<GameFullDTO>> GetListAsync(int? pageIndex = null, int? pageSize = null)
        {
            if (_uow == null)
                return null;

            var currentUser = await _currentUserProvider.GetCurrentUserAsync();
            var sidesPage = (await _uow.GetRepository<Side>()
                .GetAllPagedAsync(pageIndex, pageSize, s => s.PlayerId == currentUser.Id));
            var gamesPage = new PagedResultDTO<GameFullDTO>()
            {
                PageCount = sidesPage.PageCount,
                PageIndex = sidesPage.PageIndex,
                PageSize = sidesPage.PageSize,
                TotalDataRowsCount = sidesPage.TotalDataRowsCount,
                DataRows = mapper.Map<IEnumerable<GamePartialDTO>>(sidesPage.DataRows.Select(s => s.Game).ToList())
            };
            return gamesPage;
        }
    }
}
