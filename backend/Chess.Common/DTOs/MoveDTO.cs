namespace Chess.Common.DTOs
{
    public class MoveDTO
    {
        public int Id { get; set; }
        public GameDTO Game { get; set; }
        public int? GameId { get; set; }
        public UserDTO Player { get; set; }
        public int? PlayerId { get; set; }
        public int Ply { get; set; } // номер полухода (ход * 2)
        public string Fen { get; set; }
        public string FenAfterMove { get; set; }
        public string MoveNext { get; set; }  // сам ход
    }
}
