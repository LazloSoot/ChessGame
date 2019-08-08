using ChessGame.Core;
using NUnit.Framework;
using Chess.Common.Helpers.ChessGame;
using ChessGame.Core.Pieces.Helpers;
using CoreColor = ChessGame.Core.Moves.Helpers.Color;
using System.Collections.Generic;
using System;
using ChessGame.Test.Helpers;

namespace ChessGame.Test.ChessGame.Core
{
    [TestFixture]
    public class ChessGameTest
    {
        public static IEnumerable<TestCaseData> PromotionTestCaseData
        {
            get
            {
                yield return new TestCaseData("k1rn4/1pp3P1/p7/3b3p/2PP1B1P/P2P4/1P4p1/1K6 b - - 0 0", new PromotionTestDataWrapper()
                {
                    From = new Square("g2"),
                    To = new Square("g1"),
                    PromotedTo = Piece.BlackBishop
                });
                yield return new TestCaseData("k1rn4/1pp3P1/p7/3b3p/2PP1B1P/P2P4/1P6/1K1R3R w - - 0 0", new PromotionTestDataWrapper()
                {
                    From = new Square("g7"),
                    To = new Square("g8"),
                    PromotedTo = Piece.WhiteRook
                });
            }
        }

        [Test]
        [TestCase("r1bqkbnr/ppp2ppp/2n5/3pp1N1/4P3/3P1Q2/PPP2PPP/RNB1KB1R w KQkq - 0 0", "Qf3f7", Description = "School mate")]
        [TestCase("rnbqkbnr/ppppp2p/5p2/6p1/4P3/3P4/PPP2PPP/RNBQKBNR w KQkq - 0 0", "Qd1h5", Description = "Fulls mate")]
        [TestCase("8/4N1pk/8/8/6R1/8/8/8 w KQkq - 0 0", "Rg4h4", Description = "Anastasia's mate")]
        public void CheckMateTest(string fen, string move)
        {
            var game = new ChessGameEngine().InitGame(fen);
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            game = game.Move(move);
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(Color.Black, game.CheckTo);
            Assert.AreEqual(Color.Black, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);
        }

        [Test]
        [TestCase("7k/8/4K3/5Q2/8/8/8/8 w - - 0 0", "Qf5g6")]
        [TestCase("5k2/5P2/6K1/8/8/8/8/8 w - - 0 0", "Kg6f6")]
        public void StaleMateTest(string fen, string move)
        {
            var game = new ChessGameEngine().InitGame(fen);
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            game = game.Move(move);
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsTrue(game.IsStaleMate);
        }

        [TestCaseSource("PromotionTestCaseData")]
        [Test]
        public void PromotionTest(string fen, PromotionTestDataWrapper promotionTestdata)
        {
            var game = new ChessGameEngine().InitGame(fen);
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);
            
            var targetPiece = (Piece)game.GetPieceAt(promotionTestdata.From.X, promotionTestdata.From.Y);
            Assert.IsTrue(Enum.IsDefined(typeof(Piece), targetPiece));
            Assert.AreNotEqual(Piece.None, targetPiece);
            var pieceColor = targetPiece.GetColor();
            var expectedPawn = (pieceColor == CoreColor.White) ? Piece.WhitePawn : Piece.BlackPawn;
            Assert.AreEqual(expectedPawn, targetPiece);

            var move = $"{(char)targetPiece}{promotionTestdata.From.ToString()}{promotionTestdata.To.ToString()}{(char)promotionTestdata.PromotedTo}";
            game = game.Move(move);
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);
            
            targetPiece = (Piece)game.GetPieceAt(promotionTestdata.To.X, promotionTestdata.To.Y);
            Assert.AreNotEqual(expectedPawn, targetPiece);
            Assert.AreEqual(promotionTestdata.PromotedTo, targetPiece);
        }
    }
}
