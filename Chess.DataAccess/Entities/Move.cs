using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.DataAccess.Entities
{
    public class Move : Entity
    {
        public virtual Game Game { get; set; }
        public int? GameId { get; set; }
        public virtual Player Player { get; set; }
        public int? Player { get; set; }
        public int Ply { get; set; }
        public string Fen { get; set; } // номер полухода * 2 - 1
        public string MoveNext { get; set; }
    }
}
