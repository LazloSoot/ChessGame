using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame.Test.ChessGame.Core
{
    [TestFixture]
    public class EvaluationResults
    {
        [TestCase("2k3r1/4q3/8/8/8/8/8/R6K b - - 0 0", Description ="Black checkmates white")]
        [Test]
        public void SuccessfullyInstantCheckmateTest()
        {

        }
    }
}
