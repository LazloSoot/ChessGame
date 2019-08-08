using NUnit.Framework;
using Chess.Common.Helpers.ChessGame;
using ChessGame.Core;
using ChessGame.Core.Pieces.Helpers;
using CoreColor = ChessGame.Core.Moves.Helpers.Color;
using System;
using System.Collections.Generic;
using ChessGame.Test.Helpers;

namespace ChessGame.Test.ChessGame.Core
{
    [TestFixture]
    public class EvaluationResultsTest
    {
        public static IEnumerable<TestCaseData> PromotionTestCaseData
        {
            get
            {
                yield return new TestCaseData("k1rn4/1pp3P1/p7/3b3p/2PP1B1P/P2P4/1P4p1/1K6 b - - 0 0", new PromotionTestDataWrapper()
                {
                    From = new Square("g2"),
                    To = new Square("g1"),
                    PromotedTo = Piece.BlackQueen
                });
                yield return new TestCaseData("k1rn4/1pp3P1/p7/3b3p/2PP1B1P/P2P4/1P6/1K1R3R w - - 0 0", new PromotionTestDataWrapper()
                {
                    From = new Square("g7"),
                    To = new Square("g8"),
                    PromotedTo = Piece.WhiteQueen
                });
            }
        }


        [TestCase("2k3r1/4q3/8/8/8/8/8/R6K b - - 0 0", Color.White, 1, Description ="Black checkmates white")]
        [TestCase("6k1/1b3ppp/pb2p3/1p2P3/1P2BPnP/P1r5/1B1r3P/R2R3K b - - 0 0", Color.White, 1, Description = "Black checkmates white")]
        [TestCase("6k1/1b3ppp/p3p3/1p2P3/1P2BPnP/P1r5/1B6/2qR1N1K w - - 0 0",Color.Black, 1, Description ="White checkmates black")]
        [TestCase("r4b1r/5ppp/pb2p3/1p6/2Pq4/3P4/PP2QPPP/2k1K2R w KQ - 0 0", Color.Black, 1, Description ="White checkmates black by castling")]
        [Test]
        public void SuccessfullyInstantCheckmateTest(string fen, Color CheckMateTo, int checkmateDepth = 1)
        {
            var game = new ChessGameEngine().InitGame(fen);
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            game = game.ComputerMove();
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(CheckMateTo, game.CheckTo);
            Assert.AreEqual(CheckMateTo, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);
        }

        [Test]
        internal void PromotionTest(string fen, PromotionTestDataWrapper promotionTestdata)
        {
            var game = new ChessGameEngine().InitGame(fen);
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            var pawn = (Piece)game.GetPieceAt(promotionTestdata.From.X, promotionTestdata.From.Y);
            Assert.IsTrue(Enum.IsDefined(typeof(Piece), pawn));
            Assert.AreNotEqual(Piece.None, pawn);
            var pieceColor = pawn.GetColor();
            Assert.AreEqual((pieceColor == CoreColor.White) ? Piece.WhitePawn : Piece.BlackPawn, pawn);

            game = game.ComputerMove();
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);


            var actualPromotedTo = (Piece)game.GetPieceAt(promotionTestdata.To.X, promotionTestdata.To.Y);
            Assert.AreNotEqual((pieceColor == CoreColor.White) ? Piece.WhitePawn : Piece.BlackPawn, actualPromotedTo.ToString().ToLower().ToCharArray()[0]);
            Assert.AreEqual(promotionTestdata.PromotedTo, actualPromotedTo);
        }
        
    }
}
