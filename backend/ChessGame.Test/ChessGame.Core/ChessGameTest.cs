using ChessGame.Core;
using NUnit.Framework;
using Chess.Common.Helpers.ChessGame;
using ChessGame.Core.Pieces.Helpers;
using CoreColor = ChessGame.Core.Moves.Helpers.Color;
using System.Collections.Generic;
using System;
using ChessGame.Test.Helpers;
using System.Reflection;
using System.Linq;
using ChessGame.Core.Pieces;

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

        [Test]
        [TestCase("k1rn4/1pp2p2/p7/6Pp/2PP1B1P/P2P4/1P6/1K1R4 b - - 0 1", "pf7f5", "f6", "Pg5f6")]
        [TestCase("k1rn4/1pp2p2/p7/6Pp/2PP1p1P/P2P4/1P4P1/1K1R4 w - - 0 1", "Pg2g4", "g3", "pf4g3")]
        public void EnPassantTest(string fen, string pawnMove, string enPassantSquare, string enPassantCaptureMove)
        {
            var game = new ChessGameEngine().InitGame(new ChessGameInitSettings(fen, true, true, true));
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            Board board = (Board)game.GetType()
                .GetProperty("Board", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
            Assert.IsTrue(board.IsEnpassantRuleEnabled);

            var attackedPawnStartPosition = new Square(pawnMove.Substring(1, 2));
            var attackedPawnDestPosition = new Square(pawnMove.Substring(3, 2));
            var attackedPawn = board.GetPieceAt(attackedPawnStartPosition);
            var enPassantPosition = new Square(enPassantSquare);
            Assert.IsTrue(attackedPawn == Piece.WhitePawn || attackedPawn == Piece.BlackPawn);

            var attackingPawnStartPosition = new Square(enPassantCaptureMove.Substring(1, 2));
            var attackingPawn = board.GetPieceAt(attackingPawnStartPosition);
            Assert.IsTrue(attackingPawn == Piece.WhitePawn || attackingPawn == Piece.BlackPawn);

            game = game.Move(pawnMove);
            Assert.AreNotEqual(fenBefore, game.Fen);
            fenBefore = game.Fen;
            board = (Board)game.GetType()
                .GetProperty("Board", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
            Assert.AreEqual(enPassantSquare, board.EnPassantSquare);

            attackedPawn = board.GetPieceAt(attackedPawnDestPosition);
            Assert.AreEqual(Piece.None, board.GetPieceAt(attackedPawnStartPosition));
            Assert.AreEqual(Piece.None, board.GetPieceAt(enPassantPosition));
            Assert.IsTrue(attackedPawn == Piece.WhitePawn || attackedPawn == Piece.BlackPawn);


            game = game.Move(enPassantCaptureMove);
            Assert.AreNotEqual(fenBefore, game.Fen);
            board = (Board)game.GetType()
                .GetProperty("Board", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
            Assert.AreEqual("-", board.EnPassantSquare);

            Assert.AreEqual(Piece.None, board.GetPieceAt(attackingPawnStartPosition));
            attackingPawn = board.GetPieceAt(new Square(enPassantSquare));
            Assert.IsTrue(attackingPawn == Piece.BlackPawn || attackingPawn == Piece.WhitePawn);
            attackedPawn = board.GetPieceAt(attackedPawnDestPosition);
            Assert.AreEqual(Piece.None, attackedPawn);

        }


    }
    
}
