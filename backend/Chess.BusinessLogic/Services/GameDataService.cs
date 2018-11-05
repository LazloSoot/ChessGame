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

namespace Chess.BusinessLogic.Services
{
    public class GameDataService : CRUDService<Game, GameDTO>, IGameDataService
    {
        public GameDataService(IMapper mapper, IUnitOfWork unitOfWork)
            : base(mapper, unitOfWork)
        {

        }

        public async Task<GameDTO> CreateNewGame(GameDTO entity)
        {
            if (uow == null)
                return null;

            if (string.IsNullOrWhiteSpace(entity.Fen))
            {
                entity.Fen = ChessGame.DefaultFen;
            }
            else
            {
#warning кинуть исключение с информацией если Fen неккоректен
                // var game = new ChessGame(entity.Fen);
            }

            var game = new Game()
            {
                Fen = entity.Fen,
                Status = DataAccess.Helpers.GameStatus.Waiting,
                Sides = new List<Side>()
                {
                    new Side()
                    {
                        Color = DataAccess.Helpers.Color.Black
                    },
                    new Side()
                    {
                        Color = DataAccess.Helpers.Color.White
                    }
                }
            };

            return await base.AddAsync(entity);
        }

        public async Task<JoinGameDTO> JoinToGame(JoinGameDTO joinGameData)
        {
            if (uow == null || joinGameData.GameId < 1)
                return null;

            var targetGame = await uow.GetRepository<Game>().GetByIdAsync(joinGameData.GameId);
#warning кинуть исключение (игры нет\ нельзя подключится к игре)
            if (targetGame == null || targetGame.Status != DataAccess.Helpers.GameStatus.Waiting)
                return null;

            var sides = targetGame.Sides;
#warning кинуть исключение (цвет занят)
            var targetSide = sides.Where(s => s.Color == joinGameData.SelectedColor).First();
            if (targetSide.Player != null)
                return null;
            else
            {
#warning установить current user в качестве игрока
                
                if(sides.Where(s => s.Player != null).Count() < 1)
                {
                    targetGame.Status = DataAccess.Helpers.GameStatus.Going;
                    uow.GetRepository<Game>().Update(targetGame);
                }

                // обвновить sides, инициализировать player в joinGameData
                await uow.SaveAsync();
                return joinGameData;
            }

        }

        public Task<GameDTO> SuspendGame(int gameId)
        {
            throw new NotImplementedException();
        }
    }
}
