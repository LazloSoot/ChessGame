using Chess.DataAccess.Helpers;
using System.Collections.Generic;

namespace Chess.DataAccess.Entities
{
    public class Game : Entity
    {
        public string Fen { get; set; }
        public GameStatus Status { get; set; }
        public virtual ICollection<Move> Moves { get; set; }
        public virtual ICollection<Side> Sides { get; set; }

        public Game()
        {
            Moves = new List<Move>();
            Sides = new List<Side>();
        }

    }
}
