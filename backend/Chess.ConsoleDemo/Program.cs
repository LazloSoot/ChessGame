using System;
using ChessGame.Core;
using Chess.Common.Interfaces;

namespace Chess.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        { // 6k1/1b3ppp/pb2p3/1p2P3/1P2BPnP/P1r5/1B1rQ2P/R4R1K
            
             //var chess = new ChessGameEngine().InitGame("r4b1r/5ppp/pb2p3/1p6/2Pq4/3P4/PP2QPPP/2k1K2R w KQ - 0 0");
            var chess = new ChessGameEngine().InitGame();
            chess.RunPerfTest(4);
            // ChessGame.Check += Chess_Check;
            // ChessGame.Mate += Chess_Mate;
            while (chess.MateTo != Common.Helpers.ChessGame.Color.None || !chess.IsStaleMate)
            {
                Console.WriteLine(chess.Fen);
                Console.WriteLine(ChessToAscii(chess));
                var move = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(move))
                {
                    break;
                }
                chess = chess.Move(move);
                Console.WriteLine(chess.Fen);
                Console.WriteLine(ChessToAscii(chess));
                chess = chess.ComputerMove();
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
                    currentFigure = chess.GetPieceAt(x, y);
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
