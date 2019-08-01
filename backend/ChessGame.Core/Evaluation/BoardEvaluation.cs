using ChessGame.Core.Figures;
using ChessGame.Core.Figures.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame.Core.Evaluation
{
    internal sealed class BoardEvaluation
    {
        private int[] whitePawnInColumnCount = new int[8], 
            blackPawnInColumnCount = new int[8];
        private int bishopsCount = 0;
        internal void EvaluateBoardScore(Board board)
        {

        }
        /// <summary>
        /// Computes the single piece score.
        /// </summary>
        /// <returns>Score value</returns>
        private int EvaluatePieceScore(Board board, FigureOnSquare piece, int validMovesCount, bool isEndOfGame, ref bool insufficientMaterial)
        {
            var score = 0;
            var posX = piece.Square.X;
            score += piece.Value;
            score += piece.DefendedValue;
            score -= piece.AttackedValue;
            // If the chess piece is getting attacked and it is not protected then will consider we are about to lose it.
            // => double penalty
            if (piece.DefendedValue < piece.AttackedValue)
            {
                score -= ((piece.AttackedValue - piece.DefendedValue) * 10);
            }
            // Add score for mobility.
            score += validMovesCount;

            switch (piece.Figure)
            {
                case Figure.BlackPawn:
                case Figure.WhitePawn:
                    {
                        insufficientMaterial = false;
                        score += EvaluatePawnScore(piece);
                        break;
                    }
                case Figure.BlackKnight:
                case Figure.WhiteKnight:
                    {
                        // knights are worth less in the end game since it is difficult to mate with a knight
                        // hence they lose 10 points during the end game.
                        if (isEndOfGame)
                        {
                            score -= 10;
                        }
                        break;
                    }
                case Figure.BlackBishop:
                case Figure.WhiteBishop:
                    {
                        // Bishops are worth more in the end game, also we add a small bonus for having 2 bishops
                        // since they complement each other by controlling different ranks.
                        bishopsCount++;
                        if(bishopsCount >= 2)
                        {
                            score += 10;
                        }
                        if(isEndOfGame)
                        {
                            score += 10;
                        }
                        break;
                    }
                    // Rooks shouldnt leave their corner positions before castling has occured
                case Figure.BlackRook:
                    {
                        insufficientMaterial = false;
                        if (!board.IsBlackCastled && !((piece.Square.X == 0 && board.BlackCastlingFenPart.Contains('k')) || (piece.Square.X == 7 && board.BlackCastlingFenPart.Contains('q'))))
                        {
                            score -= 10;
                        }
                        break;
                    }
                case Figure.WhiteRook:
                    {
                        insufficientMaterial = false;
                        if (!board.IsWhiteCastled && !((piece.Square.X == 0 && board.WhiteCastlingFenPart.Contains('K')) || (piece.Square.X == 7 && board.WhiteCastlingFenPart.Contains('Q'))))
                        {
                            score -= 10;
                        }
                        break;
                    }
                case Figure.BlackQueen:
                case Figure.WhiteQueen:
                    {
                        insufficientMaterial = false;
                        break;
                    }
                case Figure.BlackKing:
                    {
                        // If he has less than 2 move, he possibly one move away from mate.
                        if (validMovesCount < 2)
                        {
                            score -= 5;
                        }
                        // penalty for losing ability to castle
                        if (!board.IsBlackCastled && string.IsNullOrWhiteSpace(board.BlackCastlingFenPart))
                        {
                            score -= 30;
                        }
                        break;
                    }
                case Figure.WhiteKing:
                    {
                        // If he has less than 2 move, he possibly one move away from mate.
                        if (validMovesCount < 2)
                        {
                            score -= 5;
                        }
                        // penalty for losing ability to castle
                        if (!board.IsWhiteCastled && string.IsNullOrWhiteSpace(board.WhiteCastlingFenPart))
                        {
                            score -= 30;
                        }
                        break;
                    }
                case Figure.None:
                default:
                    break;
            }

            // add position value
            score += piece.Figure.GetPieceSquareTableScore(piece.Square.X, piece.Square.Y, isEndOfGame);

            return score;
        }

        /// <summary>
        /// Perform evaluation for pawn: 
        /// Remove some points for pawns on the edge of the board. The idea is that since a pawn of the edge can
        /// only attack one way it is worth 15% less.
        /// Give an extra bonus for pawns that are on the 6th and 7th rank as long as they are not attacked in any way
        /// Add points based on the Pawn Piece Square Table Lookup.
        /// </summary>
        /// <returns></returns>
        private int EvaluatePawnScore(FigureOnSquare piece)
        {
            var score = 0;
            var posX = piece.Square.X;


            if (posX == 0 || posX == 7)
            {
                //Rook Pawns are worth 15% less because they can only attack one way
                score -= 15;
            }

            if (piece.Figure.GetColor() == Moves.Helpers.Color.White)
            {
                if (whitePawnInColumnCount[posX] > 0)
                {
                //Doubled Pawn
                score -= 16;
                }
                if (piece.Square.Y  == 1)
                {
                    if (piece.AttackedValue == 0)
                    {
                        whitePawnInColumnCount[posX] += 200;
                        if (piece.DefendedValue != 0)
                            whitePawnInColumnCount[posX] += 50;
                    }
                }
                else if (piece.Square.Y == 2)
                {
                    if (piece.AttackedValue == 0)
                    {
                        whitePawnInColumnCount[posX] += 100;
                        if (piece.DefendedValue != 0)
                            whitePawnInColumnCount[posX] += 25;
                    }
                }
                whitePawnInColumnCount[posX] += 10;
            }
            else
            {
                if (blackPawnInColumnCount[posX] > 0)
                {
                    //Doubled Pawn
                    score -= 16;
                }
                if (posX == 6)
                {
                    if (piece.AttackedValue == 0)
                    {
                        blackPawnInColumnCount[posX] += 200;
                        if (piece.DefendedValue != 0)
                            blackPawnInColumnCount[posX] += 50;
                    }
                }
                //Pawns in 6th Row that are not attacked are worth more points.
                else if (posX == 5)
                {
                    if (piece.AttackedValue == 0)
                    {
                        blackPawnInColumnCount[posX] += 100;
                        if (piece.DefendedValue != 0)
                            blackPawnInColumnCount[posX] += 25;
                    }
                }
                blackPawnInColumnCount[posX] += 10;
            }

            return score;
        }
    }
}
