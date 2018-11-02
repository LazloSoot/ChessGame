namespace Chess.Common.DTOs
{
    public class MoveDTO
    {
        public int Id { get; set; }
        public GameDTO Game { get; set; }
        public PlayerDTO Player { get; set; }
        public int Ply { get; set; }
        public string Fen { get; set; } // номер полухода (ход * 2 - 1)
        public string MoveNext { get; set; }
    }
}
