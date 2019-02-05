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
using System.Linq;
using Chess.Common.Interfaces;
using Chess.BusinessLogic.Services.SignalR;
using Chess.BusinessLogic.Interfaces.SignalR;

namespace Chess.BusinessLogic.Services
{
    public class GameDataService : CRUDService<Game, GameDTO>, IGameDataService
    {
        private readonly IUserService _userService;
        private readonly ISignalRNotificationService _notificationService;
        public GameDataService(
            IMapper mapper, 
            IUnitOfWork unitOfWork,
            IUserService userService,
            ISignalRNotificationService notificationService
            )
            : base(mapper, unitOfWork)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

#warning работает только с приглашением оппонента
        public async Task<GameDTO> CreateNewGameWithFriend(GameDTO game)
        {
            if (uow == null)
                return null;

            if (string.IsNullOrWhiteSpace(game.Fen))
            {
                game.Fen = ChessGame.DefaultFen;
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
            var currentUser = await _userService.GetCurrentUser();
            currentUserSide.Player = currentUser;
            game.Sides = new List<SideDTO>()
            {
                currentUserSide
            };

            GameDTO targetGame;
            var gameRepo = uow.GetRepository<Game>();
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
                targetGame = mapper.Map<GameDTO>(gameRepo.Update(createdGame));
                await uow.SaveAsync();
            }
            else
            {
                targetGame = await base.AddAsync(game);
            }
            await _notificationService.InviteUserAsync(opponent.Uid, targetGame.Id);
            return targetGame;
        }

        public async Task<GameDTO> CreateNewGameVersusAI(GameDTO game)
        {
            if (uow == null)
                return null;

            if (string.IsNullOrWhiteSpace(game.Fen))
            {
                game.Fen = ChessGame.DefaultFen;
            }

            var currentUser = await _userService.GetCurrentUser();
            var currentUserSide = game.Sides.Where(s => s.Player == null || s.PlayerId == currentUser.Id || s.Player.Id == currentUser.Id).First();
            currentUserSide.Player = currentUser;
            game.Sides = new List<SideDTO>()
            {
                currentUserSide
            };
            game.Status = DataAccess.Helpers.GameStatus.Going;
            
            return await base.AddAsync(game);
        }

        public async Task<GameDTO> JoinToGame(int gameId)
        {
            if (uow == null)
                return null;

            var targetGame = await uow.GetRepository<Game>().GetByIdAsync(gameId);
#warning кинуть исключение (игры нет\ нельзя подключится к игре)
            if (targetGame == null || targetGame.Status != DataAccess.Helpers.GameStatus.Waiting)
                return null;
            
            var hostSide = targetGame.Sides.FirstOrDefault();
#warning кинуть исключение, игра не валидна
            if (hostSide == null)
                return null;

            var color = (hostSide.Color == DataAccess.Helpers.Color.Black) ? DataAccess.Helpers.Color.White : DataAccess.Helpers.Color.Black;
            var currentDbUser = await _userService.GetCurrentUser();
            if (currentDbUser == null)
                return null;

            targetGame.Sides.Add(new Side()
            {
                Color = color,
                Player = mapper.Map<User>(currentDbUser)
            });
            targetGame.Status = DataAccess.Helpers.GameStatus.Going;
            uow.GetRepository<Game>().Update(targetGame);
            
            await uow.SaveAsync();

            await _notificationService.AcceptInvitation(hostSide.Player.Uid, targetGame.Id);
            return mapper.Map<GameDTO>(targetGame);
        }

        public Task<GameDTO> SuspendGame(int gameId)
        {
            throw new NotImplementedException();
        }
    }
}
