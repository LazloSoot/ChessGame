﻿using NUnit.Framework;
using Chess.Common.Helpers.ChessGame;
using ChessGame.Core;
using ChessGame.Core.Pieces.Helpers;
using CoreColor = ChessGame.Core.Moves.Helpers.Color;
using System;
using System.Collections.Generic;
using ChessGame.Test.Helpers;
using ChessGame.Core.Pieces;
using System.Reflection;

namespace ChessGame.Test.ChessGame.Core
{
    [TestFixture]
    public class EvaluationResultsTest
    {
        #region TestCaseData

        public static IEnumerable<TestCaseData> SuccessfulPromotionTestCaseData
        {
            get
            {
                yield return new TestCaseData("k1rn4/1pp3P1/p7/3b3p/2PP1B1P/P2P4/1P4p1/1K1RB3 b - - 0 0", new PromotionTestDataWrapper()
                {
                    From = new Square("g2"),
                    To = new Square("g1"),
                    PromotedTo = Piece.BlackQueen
                });
                yield return new TestCaseData("k1rn4/1pp3P1/p7/7p/2PP1B1P/P2P4/1P6/1K1R3R w - - 0 0", new PromotionTestDataWrapper()
                {
                    From = new Square("g7"),
                    To = new Square("g8"),
                    PromotedTo = Piece.WhiteQueen
                });
            }
        }

        public static IEnumerable<TestCaseData> UnsuccessfulPromotionTestCaseData
        {
            get
            {
                yield return new TestCaseData("k1rn4/1pp3P1/p7/3b3p/2PP1B1P/P2P4/1P4p1/1K1R4 b - - 0 0", new PromotionTestDataWrapper()
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

        public static IEnumerable<TestCaseData> EnPassantTestCaseData
        {
            get
            {
                yield return new TestCaseData(
                    new EnPassantTestDataWrapper()
                    {
                        Fen = "k7/8/8/8/5pP1/8/8/1K6 b - g3 0 1",
                        AttackingPawnPosition = new Square("f4")
                    });
                yield return new TestCaseData(
                    new EnPassantTestDataWrapper()
                    {
                        Fen = "k7/8/8/3pP3/8/8/8/1K6 w - d6 0 1",
                        AttackingPawnPosition = new Square("e5")
                    });
            }

        }
        #endregion TestCaseData


        [TestCase("2k3r1/4q3/8/8/8/8/8/R6K b - - 0 0", Color.White, 1, Description = "Black checkmates white")]
        [TestCase("6k1/1b3ppp/pb2p3/1p2P3/1P2BPnP/P1r5/1B1r3P/R2R3K b - - 0 0", Color.White, 1, Description = "Black checkmates white")]
        [TestCase("6k1/1b3ppp/p3p3/1p2P3/1P2BPnP/P1r5/1B6/2qR1N1K w - - 0 0", Color.Black, 1, Description = "White checkmates black")]
        [TestCase("r4b1r/5ppp/pb2p3/1p6/2Pq4/3P4/PP2QPPP/2k1K2R w KQ - 0 0", Color.Black, 1, Description = "White checkmates black by castling")]
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
        [TestCaseSource("SuccessfulPromotionTestCaseData")]
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

            game = game.ComputerMove();
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            // piece successfully promoted
            var actualPromotedTo = (Piece)game.GetPieceAt(promotionTestdata.To.X, promotionTestdata.To.Y);
            targetPiece = (Piece)game.GetPieceAt(promotionTestdata.To.X, promotionTestdata.To.Y);
            Assert.AreNotEqual(expectedPawn, actualPromotedTo);
            Assert.AreEqual(promotionTestdata.PromotedTo, actualPromotedTo);
        }

        [Test]
        [TestCaseSource("UnsuccessfulPromotionTestCaseData")]
        public void PromotionNoProfitTest(string fen, PromotionTestDataWrapper promotionTestdata)
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

            game = game.ComputerMove();
            var fenAfter = game.Fen;
            Assert.AreNotEqual(fenBefore, fenAfter);
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            // not profitable => no promotion
            targetPiece = (Piece)game.GetPieceAt(promotionTestdata.From.X, promotionTestdata.From.Y);
            Assert.AreEqual(expectedPawn, targetPiece);
            var pieceOnDestSquare = (Piece)game.GetPieceAt(promotionTestdata.To.X, promotionTestdata.To.Y);
            Assert.AreEqual(Piece.None, pieceOnDestSquare);
        }

        [Test]
        [TestCaseSource("EnPassantTestCaseData")]
        public void EnPassantTest(EnPassantTestDataWrapper testData)
        {
            var game = new ChessGameEngine().InitGame(new ChessGameInitSettings(testData.Fen, true, true, true));
            var fenBefore = game.Fen;
            Assert.AreEqual(Color.None, game.CheckTo);
            Assert.AreEqual(Color.None, game.MateTo);
            Assert.IsFalse(game.IsStaleMate);

            Board board = (Board)game.GetType()
                .GetProperty("Board", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
            Assert.IsTrue(board.IsEnpassantRuleEnabled);
            Assert.IsFalse(string.IsNullOrWhiteSpace(board.EnPassantSquare));

            var signY = board.MoveColor == CoreColor.White ? -1 : 1;
            var enPassantPosition = new Square(board.EnPassantSquare);
            var attackedPawnPosition = new Square(enPassantPosition.X, enPassantPosition.Y + signY);
            var attackedPawn = board.GetPieceAt(attackedPawnPosition);
            Assert.AreEqual((board.MoveColor == CoreColor.White) ? Piece.BlackPawn : Piece.WhitePawn, attackedPawn);
            var attackingPawn = board.GetPieceAt(testData.AttackingPawnPosition);
            Assert.AreEqual((board.MoveColor == CoreColor.White) ? Piece.WhitePawn : Piece.BlackPawn, attackingPawn);

            game = game.ComputerMove();
            Assert.AreNotEqual(fenBefore, game.Fen);
            fenBefore = game.Fen;
            board = (Board)game.GetType()
                .GetProperty("Board", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
            Assert.AreEqual("-", board.EnPassantSquare);
            
            Assert.AreEqual(Piece.None, board.GetPieceAt(attackedPawnPosition));
            Assert.AreEqual(Piece.None, board.GetPieceAt(testData.AttackingPawnPosition));
            Assert.AreEqual((board.MoveColor == CoreColor.White) ? Piece.BlackPawn : Piece.WhitePawn, board.GetPieceAt(enPassantPosition));
        }
    }
}
