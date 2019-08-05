﻿using ChessGame.Core.Moves;
using ChessGame.Core.Moves.Helpers;

namespace ChessGame.Core.Pieces.Helpers
{
    static class Extentions
    {
        #region Piece-Square tables

        private static readonly int[]
            pawnsSquareTable = new int[]
            {
                 0,  0,  0,  0,  0,  0,  0,  0,
                 50, 50, 50, 50, 50, 50, 50, 50,
                 10, 10, 20, 30, 30, 20, 10, 10,
                  5,  5, 10, 25, 25, 10,  5,  5,
                  0,  0,  0, 20, 20,  0,  0,  0,
                  5, -5,-10,  0,  0,-10, -5,  5,
                  5, 10, 10,-20,-20, 10, 10,  5,
                  0,  0,  0,  0,  0,  0,  0,  0
            },
            knightsSquareTable = new int[]
            {
                -50,-40,-30,-30,-30,-30,-40,-50,
                -40,-20,  0,  0,  0,  0,-20,-40,
                -30,  0, 10, 15, 15, 10,  0,-30,
                -30,  5, 15, 20, 20, 15,  5,-30,
                -30,  0, 15, 20, 20, 15,  0,-30,
                -30,  5, 10, 15, 15, 10,  5,-30,
                -40,-20,  0,  5,  5,  0,-20,-40,
                -50,-40,-30,-30,-30,-30,-40,-50
            },
            bishopsSquareTable = new int[]
            {
                -20,-10,-10,-10,-10,-10,-10,-20,
                -10,  0,  0,  0,  0,  0,  0,-10,
                -10,  0,  5, 10, 10,  5,  0,-10,
                -10,  5,  5, 10, 10,  5,  5,-10,
                -10,  0, 10, 10, 10, 10,  0,-10,
                -10, 10, 10, 10, 10, 10, 10,-10,
                -10,  5,  0,  0,  0,  0,  5,-10,
                -20,-10,-10,-10,-10,-10,-10,-20
            },
            rookSquareTable = new int[]
            {
                 0,  0,  0,  0,  0,  0,  0,  0,
                 5, 10, 10, 10, 10, 10, 10,  5,
                -5,  0,  0,  0,  0,  0,  0, -5,
                -5,  0,  0,  0,  0,  0,  0, -5,
                -5,  0,  0,  0,  0,  0,  0, -5,
                -5,  0,  0,  0,  0,  0,  0, -5,
                -5,  0,  0,  0,  0,  0,  0, -5,
                 0,  0,  0,  5,  5,  0,  0,  0
            },
            queenSquareTable = new int[]
            {
                -20,-10,-10, -5, -5,-10,-10,-20,
                -10,  0,  0,  0,  0,  0,  0,-10,
                -10,  0,  5,  5,  5,  5,  0,-10,
                 -5,  0,  5,  5,  5,  5,  0, -5,
                  0,  0,  5,  5,  5,  5,  0, -5,
                -10,  5,  5,  5,  5,  5,  0,-10,
                -10,  0,  5,  0,  0,  0,  0,-10,
                -20,-10,-10, -5, -5,-10,-10,-20
            },
            kingsEndGameSquareTable = new int[]
            {
                -50,-40,-30,-20,-20,-30,-40,-50,
                -30,-20,-10,  0,  0,-10,-20,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-30,  0,  0,  0,  0,-30,-30,
                -50,-30,-30,-30,-30,-30,-30,-50
            },
            kingsMiddleGameSquareTable = new int[]
            {
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -20,-30,-30,-40,-40,-30,-30,-20,
                -10,-20,-20,-20,-20,-20,-20,-10,
                 20, 20,  0,  0,  0,  0, 20, 20,
                 20, 30, 10,  0,  0, 10, 30, 20
            };

        #endregion

        internal static Color GetColor(this Piece piece)
        {
            if (piece == Piece.None)
                return Color.None;

            return piece.ToString()[0] == 'W' ? Color.White : Color.Black;
        }

        internal static int GetPieceValue(this Piece piece)
        {
            // source: Larry Kaufman 2012
            switch (piece)
            {
                case Piece.WhitePawn:
                case Piece.BlackPawn:
                    return 100;
                case Piece.BlackKnight:
                case Piece.WhiteKnight:
                case Piece.BlackBishop:
                case Piece.WhiteBishop:
                    return 350;
                case Piece.BlackRook:
                case Piece.WhiteRook:
                    return 525;
                case Piece.WhiteQueen:
                case Piece.BlackQueen:
                    return 1000;
                case Piece.WhiteKing:
                case Piece.BlackKing:
                    return 20000;
                case Piece.None:
                default:
                    return 0;
            }
        }

        internal static int GetPieceActionValue(this Piece piece)
        {
            switch (piece)
            {
                case Piece.WhitePawn:
                case Piece.BlackPawn:
                    return 6;
                case Piece.BlackKnight:
                case Piece.WhiteKnight:
                case Piece.BlackBishop:
                case Piece.WhiteBishop:
                    return 3;
                case Piece.BlackRook:
                case Piece.WhiteRook:
                    return 2;
                case Piece.WhiteQueen:
                case Piece.BlackQueen:
                case Piece.WhiteKing:
                case Piece.BlackKing:
                    return 1;
                case Piece.None:
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Computes points awarded for a tactical position capture by piece(piece)
        /// </summary>
        /// <remarks> 
        /// Combination of several tactical advantages for the moving side and several tactical penalties for the opponent
        /// may couse in unnecessary pawn sacrifice for set of minor tactical advantages.So always keep this in mind.</remarks>
        /// <returns>Score points</returns>
        internal static int GetPieceSquareTableScore(this Piece piece, int positionX, int positionY, bool isEndOfGame = false)
        {
            var color = piece.GetColor();
            int index;
            var position1D = positionX * 8 + positionY;
            if (color == Color.White)
            {
                index = position1D;
            }
            else
            {
                // ?????????????????????????????????????????????????????
                //index = ((position1D + 56)) - ((position1D / 8) * 16);
                index = 63 - position1D;
            }
            if (index < 0 || index > 63)
                return 0;
            switch (piece)
            {
                case Piece.WhitePawn:
                case Piece.BlackPawn:
                    return pawnsSquareTable[index];
                case Piece.BlackKnight:
                case Piece.WhiteKnight:
                    return knightsSquareTable[index];
                case Piece.BlackBishop:
                case Piece.WhiteBishop:
                    return bishopsSquareTable[index];
                case Piece.BlackRook:
                case Piece.WhiteRook:
                    return rookSquareTable[index];
                case Piece.WhiteQueen:
                case Piece.BlackQueen:
                    return queenSquareTable[index];
                case Piece.WhiteKing:
                case Piece.BlackKing:
                    {
                        if (isEndOfGame)
                        {
                            return kingsEndGameSquareTable[index];
                        }
                        else
                        {
                            return kingsMiddleGameSquareTable[index];
                        }
                    }
                case Piece.None:
                default:
                    return 0;
            }
        }

        #region Board

#warning TEST REQUARED!!!111
        internal static bool CanKingCastle(this Board board, bool isToKingside)
        {
            if (board.MoveColor == Color.White && board.IsWhiteCastled
                || board.MoveColor == Color.Black && board.IsBlackCastled)
                return false;
            board.MoveColor = board.MoveColor.FlipColor();
            if (board.IsCheckTo())
            {
                board.MoveColor = board.MoveColor.FlipColor();
                return false;
            }
            board.MoveColor = board.MoveColor.FlipColor();
            var isWhiteSide = board.MoveColor == Moves.Helpers.Color.White;
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
                if (board.GetPieceAt(1, y) != Piece.None ||
                    board.GetPieceAt(2, y) != Piece.None ||
                    board.GetPieceAt(3, y) != Piece.None)
                {
                    return false;
                }
            }
            else
            {
                if (board.GetPieceAt(6, y) != Piece.None ||
                    board.GetPieceAt(5, y) != Piece.None)
                {
                    return false;
                }
            }
            var firstKingDestSquare = new Square(4 + stepX, y);
            mf = new MovingPiece(new PieceOnSquare(king, new Square(4, y)), firstKingDestSquare);
            var currentMove = new Move(board);
            if (!currentMove.CanMove(mf))
                return false;
            if (board.IsCheckAfterMove(mf))
                return false;

            var boardAfterFirstMove = board.GetBoardAfterFirstKingCastlingMove(mf);
            var moveAfterFirstKingMove = new Move(boardAfterFirstMove);
            var finalKingDestSquare = new Square(firstKingDestSquare.X + stepX, y);
            mf = new MovingPiece(new PieceOnSquare(king, firstKingDestSquare), finalKingDestSquare);
            if (!moveAfterFirstKingMove.CanMove(mf))
                return false;
            if (boardAfterFirstMove.IsCheckAfterMove(mf))
                return false;

            return true;

            bool IsCastlingPossible(bool isKingside, Color color)
            {
                var currentCastrlingFenPart = ((color == Color.White) ? board.WhiteCastlingFenPart : board.BlackCastlingFenPart).ToLower();
                return (isKingside) ? currentCastrlingFenPart.Contains('k') : currentCastrlingFenPart.Contains('q');
            }
        }

        #endregion Board
    }
}
