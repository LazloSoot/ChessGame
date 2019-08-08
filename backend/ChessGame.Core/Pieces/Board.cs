using ChessGame.Core.Moves;
using ChessGame.Core.Moves.Helpers;
using ChessGame.Core.Pieces.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessGame.Core.Pieces
{
    internal sealed class Board : IComparable<Board>
    {
        private Piece[,] pieces;

        internal string Fen { get; private set; }

        internal string WhiteCastlingFenPart { get; private set; }

        internal string BlackCastlingFenPart { get; private set; }

        internal Color MoveColor { get; set; }

        internal int MoveNumber { get; set; }
        /// <summary>
        /// Computed by increasing better positions for White and decreasing for better positions for Black.
        /// Black is always trying to find boards with the lowest score and White with the highest.
        /// </summary>
        internal int Score { get; set; }
        /// <summary>
        /// This flag is needed for evaluation function, to give bonuse score for castling.
        /// </summary>
        internal bool IsWhiteCastled { get; set; }
        /// <summary>
        /// This flag is needed for evaluation function, to give bonuse score for castling.
        /// </summary>
        internal bool IsBlackCastled { get; set; }
        /// <summary>
        /// Enables fifty-move rule.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Fifty-move_rule</remarks>
        internal bool IsFiftyMovesRuleEnabled { get; set; }
        /// <summary>
        /// Enables En passant capture rule.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/En_passant</remarks>
        internal bool IsEnpassantRuleEnabled { get; set; }
        /// <summary>
        /// Enables threefold repetition rule (also known as repetition of position).
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Threefold_repetition</remarks>
        internal bool IsThreefoldRepetitionRuleEnabled { get; set; }
        /// <summary>
        /// Counter to check is fifty moves rule condition satisfied.
        /// </summary>
        internal int FiftyMovesCount { get; private set; }
        /// <summary>
        /// Counter to check is three moves repition rule condition satisfied.
        /// </summary>
        internal int RepeatedMovesCount { get; set; }
        internal Color MateTo { get; set; } = Color.None;
        internal Color CheckTo { get; set; } = Color.None;
        internal bool IsStaleMate { get; set; } = false;
        internal bool IsInsufficientMaterial { get; set; } = false;
        internal bool IsEndOfGamePhase { get; set; } = false;

        internal Board(string fen, int repeatedMovesCount = 0)
        {
            Fen = fen;
            RepeatedMovesCount = repeatedMovesCount < 0 ? 0 : repeatedMovesCount;
            pieces = new Piece[8, 8];
            InitPiecesPosition();
        }

        internal Piece GetPieceAt(Square square)
        {
            return pieces[square.X, square.Y];
        }

        internal Piece GetPieceAt(int x, int y)
        {
            return pieces[x, y];
        }

        internal Board Move(MovingPiece mf)
        {
            var nextBoardState = new Board(Fen);
            
            nextBoardState.SetPieceAt(mf.From, Piece.None);
            nextBoardState.SetPieceAt(mf.To, mf.Promotion == Piece.None ? mf.Piece : mf.Promotion);

            if (MoveColor == Color.Black)
                nextBoardState.MoveNumber = MoveNumber + 1;

            nextBoardState.MoveColor = MoveColor.FlipColor();
            nextBoardState.UpdateCastlingData(mf);
            nextBoardState.GenerateNextFen();
            return nextBoardState;
        }

        /// <summary>
        /// Checks board for check, checkmate and stalemate
        /// </summary>
        internal void CheckBoard()
        {
            if((IsFiftyMovesRuleEnabled && FiftyMovesCount >= 50) || (IsThreefoldRepetitionRuleEnabled && RepeatedMovesCount >= 3))
            {
                IsStaleMate = true;
                return;
            }

            var targetColor = MoveColor;
            MoveColor = MoveColor.FlipColor();
            if (IsCheckToOpponent())
            {
                CheckTo = targetColor;
                
                if (!IsMoveAvailable(targetColor))
                {
                    MateTo = targetColor;
                }
            }
            else if (!IsMoveAvailable(targetColor))
            {
                IsStaleMate = true;
            }
            MoveColor = MoveColor.FlipColor();
        }

        private Board Castle(MovingPiece king, MovingPiece rook)
        {
            var nextBoardState = new Board(Fen);

            nextBoardState.SetPieceAt(king.From, Piece.None);
            nextBoardState.SetPieceAt(king.To, king.Piece);
            nextBoardState.SetPieceAt(rook.From, Piece.None);
            nextBoardState.SetPieceAt(rook.To, rook.Piece);

            if (MoveColor == Color.Black)
            {
                nextBoardState.MoveNumber = MoveNumber + 1;
                nextBoardState.IsBlackCastled = true;
            } else
            {
                nextBoardState.IsWhiteCastled = true;
            }

            nextBoardState.MoveColor = MoveColor.FlipColor();
            nextBoardState.UpdateCastlingData(king);
            nextBoardState.GenerateNextFen();
            return nextBoardState;
        }

        private void UpdateCastlingData(MovingPiece mf)
        {
            var targetColor = mf.Piece.GetColor();
            var currentCastlingFenPart = (targetColor == Color.White) ? WhiteCastlingFenPart : BlackCastlingFenPart;
            if (string.IsNullOrWhiteSpace(currentCastlingFenPart))
                return;

            switch (mf.Piece)
            {
                case (Piece.BlackRook):
                    {
                        if(mf.From.X == 7)
                        {
                            BlackCastlingFenPart = (BlackCastlingFenPart.Contains('q')) ? "q" : "";
                        } else 
                        if(mf.From.X == 0)
                        {
                            BlackCastlingFenPart = (BlackCastlingFenPart.Contains('k')) ? "k" : "";
                        }
                        break;
                    }
                case (Piece.WhiteRook):
                    {
                        if (mf.From.X == 7)
                        {
                            WhiteCastlingFenPart = (WhiteCastlingFenPart.Contains('Q')) ? "Q" : "";
                        }
                        else
                        if (mf.From.X == 0)
                        {
                            WhiteCastlingFenPart = (WhiteCastlingFenPart.Contains('K')) ? "K" : "";
                        }
                        break;
                    }
                case (Piece.WhiteKing):
                    {
                        WhiteCastlingFenPart = string.Empty;
                        break;
                    }
                case (Piece.BlackKing):
                    {
                        BlackCastlingFenPart = string.Empty;
                        break;
                    }
            }
        }

        internal Board GetBoardAfterFirstKingCastlingMove(MovingPiece king)
        {
            var nextBoardState = new Board(Fen);
            nextBoardState.SetPieceAt(king.From, Piece.None);
            nextBoardState.SetPieceAt(king.To, king.Piece);
            nextBoardState.GenerateNextFen();
            return nextBoardState;
        }

        internal IEnumerable<PieceOnSquare> YieldPieces()
        {
            foreach (var s in Square.YieldSquares())
            {
                if (GetPieceAt(s).GetColor() == MoveColor)
                    yield return new PieceOnSquare(GetPieceAt(s), s);
            }
        }

        internal bool IsIGotCheckAfterMove(MovingPiece movingPiece)
        {
            var boardAfterMove = Move(movingPiece);
            return boardAfterMove.IsCheckToOpponent();
        }

        internal bool IsCheckToOpponent()
        {
            Square targetKingPosition = FindTargetKingPosition();
            var move = new Move(this);
            MovingPiece currentMovingPiece;
            foreach (var pieceOnSquare in YieldPieces())
            {
                currentMovingPiece = new MovingPiece(pieceOnSquare, targetKingPosition);
                if (move.CanMove(currentMovingPiece))
                    return true;
            }

            return false;
        }
        
        internal bool CanKingCastle(bool isToKingside)
        {
            if (MoveColor == Color.White && IsWhiteCastled
                || MoveColor == Color.Black && IsBlackCastled)
                return false;
            MoveColor = MoveColor.FlipColor();
            if (IsCheckToOpponent())
            {
                MoveColor = MoveColor.FlipColor();
                return false;
            }
            MoveColor = MoveColor.FlipColor();
            var isWhiteSide = MoveColor == Moves.Helpers.Color.White;
            var king = (isWhiteSide) ? Piece.WhiteKing : Piece.BlackKing;
            var rookPiece = (isWhiteSide) ? Piece.WhiteRook : Piece.BlackRook;
            var y = (isWhiteSide) ? 0 : 7;
            var stepX = (isToKingside) ? 1 : -1;
            if (!IsCastlingPossible(stepX > 0, king.GetColor()))
            {
                return false;
            }
            MovingPiece mf;

            if (stepX == -1)
            {
                if (GetPieceAt(1, y) != Piece.None ||
                    GetPieceAt(2, y) != Piece.None ||
                    GetPieceAt(3, y) != Piece.None)
                {
                    return false;
                }
            }
            else
            {
                if (GetPieceAt(6, y) != Piece.None ||
                    GetPieceAt(5, y) != Piece.None)
                {
                    return false;
                }
            }
            var firstKingDestSquare = new Square(4 + stepX, y);
            mf = new MovingPiece(new PieceOnSquare(king, new Square(4, y)), firstKingDestSquare);
            var currentMove = new Move(this);
            if (!currentMove.CanMove(mf))
                return false;
            if (IsIGotCheckAfterMove(mf))
                return false;

            var boardAfterFirstMove = GetBoardAfterFirstKingCastlingMove(mf);
            var moveAfterFirstKingMove = new Move(boardAfterFirstMove);
            var finalKingDestSquare = new Square(firstKingDestSquare.X + stepX, y);
            mf = new MovingPiece(new PieceOnSquare(king, firstKingDestSquare), finalKingDestSquare);
            if (!moveAfterFirstKingMove.CanMove(mf))
                return false;
            if (boardAfterFirstMove.IsIGotCheckAfterMove(mf))
                return false;

            return true;

            bool IsCastlingPossible(bool isKingside, Color color)
            {
                var currentCastrlingFenPart = ((color == Color.White) ? WhiteCastlingFenPart : BlackCastlingFenPart).ToLower();
                return (isKingside) ? currentCastrlingFenPart.Contains('k') : currentCastrlingFenPart.Contains('q');
            }
        }

        internal Board Castle(bool isToKingside)
        {
            var isWhiteSide = MoveColor == Color.White;
            var king = (isWhiteSide) ? Piece.WhiteKing : Piece.BlackKing;
            var rookPiece = (isWhiteSide) ? Piece.WhiteRook : Piece.BlackRook;
            var y = (isWhiteSide) ? 0 : 7;
            var stepX = (isToKingside) ? 1 : -1;
            PieceOnSquare rook;

            if (stepX == -1)
            {
                rook = new PieceOnSquare(rookPiece, new Square(0, y));
            }
            else
            {
                rook = new PieceOnSquare(rookPiece, new Square(7, y));
            }
            var firstKingDestSquare = new Square(4 + stepX, y);
            var finalKingDestSquare = new Square(firstKingDestSquare.X + stepX, y);

            return Castle(new MovingPiece(new PieceOnSquare(king, new Square(4, y)), finalKingDestSquare), new MovingPiece(rook, firstKingDestSquare));
        }
        /// <summary>
        /// Tries to find at least one available move.Useful to check on checkmate/stalemate situation.
        /// </summary>
        /// <returns></returns>
        internal bool IsMoveAvailable(Color movesAvailableFor = Color.None)
        {
            if (movesAvailableFor == Color.None)
                movesAvailableFor = MoveColor;
            var currentColor = MoveColor;
            MoveColor = movesAvailableFor;
            var currentMove = new Move(this);
            var allMoves = new List<MovingPiece>();
            MovingPiece movingPiece;
            foreach (var pieceOnSquare in YieldPieces())
            {
                foreach (var squareTo in Square.YieldSquares())
                {
                    movingPiece = new MovingPiece(pieceOnSquare, squareTo);
                    if (currentMove.CanMove(movingPiece) &&
                        !IsIGotCheckAfterMove(movingPiece))
                    {
                        MoveColor = currentColor;
                        return true;
                    }
                }
            }

            MoveColor = currentColor;
            return false;
        }

        internal Board FastCopy()
        {
            return MemberwiseClone() as Board;
        }

        private Square FindTargetKingPosition()
        {
            var targetPiece = MoveColor == Color.Black ? Piece.WhiteKing : Piece.BlackKing;
            foreach (var square in Square.YieldSquares())
            {
                if (GetPieceAt(square) == targetPiece)
                    return square;
            }
            return default(Square);
        }

        private void GenerateNextFen()
        {

            var piecesBldr = new StringBuilder();
            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                {
                    piecesBldr.Append(pieces[x, y] == Piece.None ? '1' : (char)pieces[x, y]);
                }
                if (y > 0)
                    piecesBldr.Append('/');
            }
            var eight = "11111111";
            for (int i = 8; i >= 2; i--)
            {
                piecesBldr.Replace(eight.Substring(0, i), i.ToString());
            }
            //  "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            var castlingFenPart = (string.IsNullOrEmpty(WhiteCastlingFenPart) && string.IsNullOrEmpty(BlackCastlingFenPart)) ? "-" : $"{WhiteCastlingFenPart}{BlackCastlingFenPart}";
            Fen = piecesBldr.Append($" {(char)MoveColor} {castlingFenPart} - 0 {MoveNumber}").ToString();
        }

        private void InitPiecesPosition()
        {

#warning INIT FIFTY MOVES COUNT
            string[] parts = Fen.Split();
#warning Fen can be length of 4
            if (parts.Length < 6)
                return;
            var castlingFenPart = parts[2];
            WhiteCastlingFenPart = new string(castlingFenPart.Where(c => char.IsUpper(c)).ToArray());
            BlackCastlingFenPart = new string(castlingFenPart.Where(c => char.IsLower(c)).ToArray());

            InitPieces(parts[0]);
            MoveColor = string.Equals("b", parts[1].Trim().ToLower()) ? Color.Black : Color.White;
            MoveNumber = int.Parse(parts[5]);

            void InitPieces(string data)
            {
                for (int i = 8; i >= 2; i--)
                {
                    data = data.Replace(i.ToString(), (i - 1).ToString() + "1");
                }

                var lines = data.Split('/');
                for (int y = 7; y >= 0; y--)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        pieces[x, y] = (Piece)lines[7 - y][x];
                    }
                }
            }
        }

        private void SetPieceAt(Square square, Piece piece)
        {
            pieces[square.X, square.Y] = piece;

        }

        public int CompareTo(Board other)
        {
            return other.Score.CompareTo(Score);
        }
    }
}
