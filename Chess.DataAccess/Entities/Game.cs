namespace Chess.DataAccess.Entities
{
    public class Game : Entity
    {
        public string Fen { get; set; }
        public GameStatus Status { get; set; }
    }
}
