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
using Chess.BusinessLogic.Helpers;

namespace Chess.BusinessLogic.Services
{
    public class GameDataService : CRUDService<Game, GameFullDTO>, IGameDataService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISignalRNotificationService _notificationService;
        public GameDataService(
            IMapper mapper, 
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider,
            ISignalRNotificationService notificationService
            )
            : base(mapper, unitOfWork)
        {
            _notificationService = notificationService;
            _currentUserProvider = currentUserProvider;
        }

#warning работает только с приглашением оппонента
        public async Task<GameFullDTO> CreateNewGameWithFriend(GameFullDTO game)
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
            var currentUser = await _currentUserProvider.GetCurrentUserAsync();
            currentUserSide.Player = mapper.Map<UserDTO>(currentUser);
            game.Sides = new List<SideDTO>()
            {
                currentUserSide
            };

            GameFullDTO targetGame;
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
                targetGame = mapper.Map<GameFullDTO>(gameRepo.Update(createdGame));
                await uow.SaveAsync();
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
            if (uow == null)
                return null;

            if (string.IsNullOrWhiteSpace(game.Fen))
            {
                game.Fen = ChessGame.DefaultFen;
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
            var currentDbUser = await _currentUserProvider.GetCurrentUserAsync();
            if (currentDbUser == null)
                return null;

            targetGame.Sides.Add(new Side()
            {
                Color = color,
                Player = currentDbUser
            });
            targetGame.Status = DataAccess.Helpers.GameStatus.Going;
            uow.GetRepository<Game>().Update(targetGame);
            
            await uow.SaveAsync();

            await _notificationService.AcceptInvitation(hostSide.Player.Uid, targetGame.Id);
            return mapper.Map<GameFullDTO>(targetGame);
        }

        public Task<GameFullDTO> SuspendGame(int gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GameWidthConclusionDTO>> GetUserGames(int userID, int? pageIndex, int? pageSize)
        {
            if (uow == null)
                return null;

            var user = await uow.GetRepository<User>().GetByIdAsync(userID);
            if (user == null)
                return null;

            var games = (await uow.GetRepository<Side>()
                .GetAllAsync(pageIndex, pageSize, s => s.PlayerId == user.Id))
                .Select(s => s.Game);
            var initializedGamesDtos = mapper
                .Map<IEnumerable<Game>, IEnumerable<GameWidthConclusionDTO>>(games, opt => opt.Items["ConclusionForUserId"] = user.Id);
            return initializedGamesDtos;
        } 
        public override async Task<IEnumerable<GameFullDTO>> GetListAsync(int? pageIndex = null, int? pageSize = null)
        {
            if (uow == null)
                return null;

            var currentUser = await _currentUserProvider.GetCurrentUserAsync();
            var games = (await uow.GetRepository<Side>()
                .GetAllAsync(pageIndex, pageSize, s => s.PlayerId == currentUser.Id))
                .Select(s => s.Game);
            return mapper.Map<IEnumerable<GamePartialDTO>>(games);
        }
    }
}
