using ChessGame.Core;
using NUnit.Framework;
using Chess.Common.Helpers.ChessGame;

namespace ChessGame.Test.ChessGame.Core
{
    [TestFixture]
    public class ChessGame
    {
        [Test]
        [TestCase("r1bqkbnr/ppp2ppp/2n5/3pp1N1/4P3/3P1Q2/PPP2PPP/RNB1KB1R w KQkq - 0 0", "Qf3f7", Description ="School mate")]
        [TestCase("rnbqkbnr/ppppp2p/5p2/6p1/4P3/3P4/PPP2PPP/RNBQKBNR w KQkq - 0 0", "Qd1h5", Description ="Fulls mate")]
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
    }
}
