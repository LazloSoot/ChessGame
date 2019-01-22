using System;
using Chess.BL;
using Chess.Common.Interfaces;

namespace Chess.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var chess = new ChessGame().InitGame("r3k2r/p2pp2p/8/8/8/8/P2PP2P/R2QK2R w KQkq - 0 1");
            ChessGame.Check += Chess_Check;
            ChessGame.Mate += Chess_Mate;
            while (true)
            {
                Console.WriteLine(chess.Fen);
                Console.WriteLine(ChessToAscii(chess));
                var move = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(move))
                {
                    break;
                }
                chess = chess.Move(move);
            }
        }

        private static void Chess_Mate(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Mate! Game over.");
            Console.ResetColor();
        }

        private static void Chess_Check(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Check!");
            Console.ResetColor();
        }

        static string ChessToAscii(IChessGame chess)
        {
            var text = "  +----------------+\n";
            char currentFigure;
            for (int y = 7; y >= 0; y--)
            {
                text += y + 1;
                text += " | ";
                for (int x = 0; x < 8; x++)
                {
                    currentFigure = chess.GetFigureAt(x, y);
                    text += ((currentFigure == '1') ? '.' : currentFigure) + " ";
                }
                text += "|\n";
            }
            text += "  +---------------+\n";
            text += "    a b c d e f g h\n";
            return text;
        }
    }
}
