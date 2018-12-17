using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.DataAccess.Entities
{
    public class Move : Entity
    {
        public virtual Game Game { get; set; }
        public int? GameId { get; set; }
        public virtual User Player { get; set; }
        public int? PlayerId { get; set; }
        public int Ply { get; set; }
        public string Fen { get; set; } // номер полухода (ход * 2 - 1)
        public string MoveNext { get; set; }
    }
}
