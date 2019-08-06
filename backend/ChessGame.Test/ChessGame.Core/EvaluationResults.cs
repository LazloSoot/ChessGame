using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Chess.Common.Helpers.ChessGame;
using ChessGame.Core;

namespace ChessGame.Test.ChessGame.Core
{
    [TestFixture]
    public class EvaluationResults
    {
        [TestCase("2k3r1/4q3/8/8/8/8/8/R6K b - - 0 0", Color.White, 1, Description ="Black checkmates white")]
        [TestCase("6k1/1b3ppp/pb2p3/1p2P3/1P2BPnP/P1r5/1B1r3P/R2R3K b - - 0 0", Color.White, 1, Description = "Black checkmates white")]
        [TestCase("6k1/1b3ppp/p3p3/1p2P3/1P2BPnP/P1r5/1B6/2qR1N1K w - - 0 0",Color.Black, 1, Description ="White checkmates black")]
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
    }
}
