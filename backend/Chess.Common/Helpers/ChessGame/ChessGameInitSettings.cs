namespace Chess.Common.Helpers.ChessGame
{
    public struct ChessGameInitSettings
    {
        public string Fen { get; private set; }
        /// <summary>
        /// Enables fifty-move rule.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Fifty-move_rule</remarks>
        public bool IsFiftyMovesRuleEnabled { get; private  set; }
        /// <summary>
        /// Enables threefold repetition rule (also known as repetition of position).
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Threefold_repetition</remarks>
        public bool IsThreefoldRepetitionRuleEnabled { get; private set; }
        /// <summary>
        /// Enables En passant capture rule.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/En_passant</remarks>
        public bool IsEnpassantRuleEnabled { get; private set; }
        /// <summary>
        /// This flag gives a bonus score for castling.
        /// </summary>
        public bool IsWhiteCastled { get; set; }
        /// <summary>
        /// This flag gives a bonus score for castling.
        /// </summary>
        public bool IsBlackCastled { get; set; }
        /// <summary>
        /// Repeated moves, according to threeforse repetion rule.
        /// </summary>
        public int RepeatedMovesCount { get; set; }
        public ChessGameInitSettings(string fen, bool isFiftyMovesRuleEnabled = false, bool isEnPassantRuleEnabled = false, bool isThreeRepeatedMovesRuleEnabled = false)
        {
            Fen = fen;
            IsFiftyMovesRuleEnabled = isFiftyMovesRuleEnabled;
            IsEnpassantRuleEnabled = isEnPassantRuleEnabled;
            IsThreefoldRepetitionRuleEnabled = isThreeRepeatedMovesRuleEnabled;
            RepeatedMovesCount = 0;
            IsBlackCastled = false;
            IsWhiteCastled = false;
        }
    }
}
