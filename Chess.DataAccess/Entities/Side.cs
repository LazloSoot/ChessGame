using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.DataAccess.Entities
{
    public class Side : Entity
    {
        public virtual Game Game { get; set; }
        public int? GameId { get; set; }
        public virtual Player Player { get; set; }
        public int? PlayerId { get; set; }
        public Color Color { get; set; }
        public int Points { get; set; }
        public bool IsDraw { get; set; }
        public bool IsResign { get; set; }
    }
}
