using ChessGame.Core.Pieces.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame.Test.Helpers
{
    /// <summary>
    ///  Square and Piece are internal members and using them as a parameter leads to inconsistent accessibility error
    /// </summary>
    public class PromotionTestDataWrapper
    {
        internal Square From { get; set; }
        internal Square To { get; set; }
        internal Piece PromotedTo { get; set; }
    }

    public class EnPassantTestDataWrapper
    {
        public string Fen { get; set; }
        internal Square AttackingPawnPosition { get; set; }
    }
}
