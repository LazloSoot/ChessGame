using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Common.DTOs
{
    public class MoveRequest
    {
        public int Id { get; set; }
        public string Move { get; set; }
        public int GameId { get; set; }
    }
}
