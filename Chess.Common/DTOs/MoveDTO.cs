namespace Chess.Common.DTOs
{
    public class MoveDTO
    {
        public int Id { get; set; }
        public GameDTO Game { get; set; }
        public int? GameId { get; set; }
        public PlayerDTO Player { get; set; }
        public int? PlayerId { get; set; }
        public int Ply { get; set; } // номер полухода (ход * 2 - 1)
        public string Fen { get; set; } // состояние до хода 
        public string MoveNext { get; set; }  // сам ход
    }
}
