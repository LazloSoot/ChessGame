using ChessGame.Core.Pieces;
using ChessGame.Core.Pieces.Helpers;
using ChessGame.Core.Moves;
using System;
using System.Collections.Generic;
using System.Text;
using ChessGame.Core.Moves.Helpers;

namespace ChessGame.Core.Evaluation
{
    internal sealed class BoardEvaluation
    {
        static int AlphaBetaInvokeCount = 0;
        private int[] whitePawnInColumnCount = new int[8],
            blackPawnInColumnCount = new int[8];
        internal void EvaluateBoardScore(Board board)
        {
            board.Score = 0;
            // to check Insufficient Material Tie Rule condition
            var insufficientMaterial = true;

            #region BoardEventsScore

            if (board.IsStaleMate
                || (board.IsThreefoldRepetitionRuleEnabled && board.RepeatedMovesCount >= 3)
                || (board.IsFiftyMovesRuleEnabled && board.FiftyMovesCount >= 50))
            {
                return;
            }

            switch (board.CheckTo)
            {
                case Color.White:
                    {
                        board.Score -= 75;
                        if (board.IsEndOfGamePhase)
                        {
                            board.Score -= 10;
                        }
                        break;
                    }
                case Color.Black:
                    {
                        board.Score += 75;
                        if (board.IsEndOfGamePhase)
                        {
                            board.Score += 10;
                        }
                        break;
                    }
                case Color.None:
                default:
                    break;
            }
            switch (board.MateTo)
            {
                case Color.White:
                    {
                        board.Score = -32767;
                        break;
                    }
                case Color.Black:
                    {
                        board.Score = 32767;
                        break;
                    }
                case Color.None:
                default:
                    break;
            }

            if (board.IsWhiteCastled)
            {
                board.Score += 40;
            }
            if (board.IsBlackCastled)
            {
                board.Score -= 40;
            }
            if (board.MoveColor == Color.White)
            {
                board.Score += 10;
            }
            else
            {
                board.Score -= 10;
            }

            #endregion BoardEventsScore

            #region PiecesScore

            // to add bonus for dubled bishops and if there are 2 knight we cant call unsuffitial materail tie
            // Paradoxically, although the king and two knights cannot force checkmate of the lone king, 
            // there are positions in which the king and two knights can force checkmate against a king and some additional material
            var blackBishopsCount = 0;
            var whiteBishopsCount = 0;
            // to check insuffisient material tie rule
            // king and n * bishop (n > 0) on the same color versus king = draw
            // king and n * bishop (n > 0) versus king and m * bishops (m > 0) with all bishops on same color = draw
            var bishopsOnWhiteSquareCount = 0;
            var knightsCount = 0;
            var allPieces = new Dictionary<Square, PieceOnSquare>();

            MovingPiece movingPiece = null;
            var move = new Move(board);

            /// enumerates all pieces, checks all valid moves and initializes special values for evaluation
            InitAllPieces(allPieces);
            /// move validation and checkmate validation is depended on current turn color, hence we should flip color 
            /// to validate it for both colors
            board.MoveColor = board.MoveColor.FlipColor();
            InitAllPieces(allPieces);
            board.MoveColor = board.MoveColor.FlipColor();

            foreach (var piece in allPieces.Values)
            {
                board.Score += EvaluatePieceScore(board, piece, board.IsEndOfGamePhase, ref insufficientMaterial, ref (piece.Piece.GetColor() == Moves.Helpers.Color.White ? ref whiteBishopsCount : ref blackBishopsCount), ref bishopsOnWhiteSquareCount, ref knightsCount);
            }

            if (whiteBishopsCount + blackBishopsCount > 1 && (whiteBishopsCount + blackBishopsCount != bishopsOnWhiteSquareCount))
            {
                insufficientMaterial = false;
            }


            /// Adds to inputed dictionary all pieces depend on current turn color, inits their Attacked/Defended values 
            /// and cheks all valid moves for each piece
            void InitAllPieces(Dictionary<Square, PieceOnSquare> pieces)
            {
                PieceOnSquare currentCapturedPiece, currentCapturedPieceInDictionary;
                foreach (var pieceOnSquare in board.YieldPieces())
                {
                    if (!pieces.ContainsKey(pieceOnSquare.Square))
                    {
                        pieces.Add(pieceOnSquare.Square, pieceOnSquare);
                    }
                    foreach (var squareTo in Square.YieldSquares())
                    {
                        movingPiece = new MovingPiece(pieceOnSquare, squareTo);
                        if (move.CanMove(movingPiece, out currentCapturedPiece) &&
                            !board.IsCheckAfterMove(movingPiece))
                        {
                            if (currentCapturedPiece != null)
                            {
                                if (!pieces.ContainsKey(currentCapturedPiece.Square))
                                {
                                    pieces.Add(currentCapturedPiece.Square, currentCapturedPiece);
                                }
                                else
                                {
                                    currentCapturedPieceInDictionary = pieces[currentCapturedPiece.Square];
                                    currentCapturedPieceInDictionary.AttackedValue += currentCapturedPiece.AttackedValue;
                                    currentCapturedPieceInDictionary.DefendedValue += currentCapturedPiece.DefendedValue;
                                }
                            }
                            pieceOnSquare.ValidMovesCount++;
                        }
                        else if (movingPiece.IsItCastlingMove())
                        {
                            var targetColor = movingPiece.Piece.GetColor();
                            if (targetColor != board.MoveColor)
                                continue;
                            var isToKingside = movingPiece.SignX > 0;
                            if (board.CanKingCastle(isToKingside))
                            {
                                pieceOnSquare.ValidMovesCount++;
                            }
                        }
                    }
                }
            }

            #endregion PiecesScore

            #region BoardLevelEventsHandling

            if (insufficientMaterial)
            {
                board.Score = 0;
                board.IsStaleMate = true;
                board.IsInsufficientMaterial = true;
                return;
            }
            if (allPieces.Count < 10)
            {
                board.IsEndOfGamePhase = true;
            }

            #endregion BoardLevelEventsHandling

            #region DoubledIsolatedPawns

            //Black Isolated Pawns
            if (blackPawnInColumnCount[0] >= 1 && blackPawnInColumnCount[1] == 0)
            {
                board.Score += 12;
            }
            if (blackPawnInColumnCount[1] >= 1 && blackPawnInColumnCount[0] == 0 &&
            blackPawnInColumnCount[2] == 0)
            {
                board.Score += 14;
            }
            if (blackPawnInColumnCount[2] >= 1 && blackPawnInColumnCount[1] == 0 &&
            blackPawnInColumnCount[3] == 0)
            {
                board.Score += 16;
            }
            if (blackPawnInColumnCount[3] >= 1 && blackPawnInColumnCount[2] == 0 &&
            blackPawnInColumnCount[4] == 0)
            {
                board.Score += 20;
            }
            if (blackPawnInColumnCount[4] >= 1 && blackPawnInColumnCount[3] == 0 &&
            blackPawnInColumnCount[5] == 0)
            {
                board.Score += 20;
            }
            if (blackPawnInColumnCount[5] >= 1 && blackPawnInColumnCount[4] == 0 &&
            blackPawnInColumnCount[6] == 0)
            {
                board.Score += 16;
            }
            if (blackPawnInColumnCount[6] >= 1 && blackPawnInColumnCount[5] == 0 &&
            blackPawnInColumnCount[7] == 0)
            {
                board.Score += 14;
            }
            if (blackPawnInColumnCount[7] >= 1 && blackPawnInColumnCount[6] == 0)
            {
                board.Score += 12;
            }
            //White Isolated Pawns
            if (whitePawnInColumnCount[0] >= 1 && whitePawnInColumnCount[1] == 0)
            {
                board.Score -= 12;
            }
            if (whitePawnInColumnCount[1] >= 1 && whitePawnInColumnCount[0] == 0 &&
            whitePawnInColumnCount[2] == 0)
            {
                board.Score -= 14;
            }
            if (whitePawnInColumnCount[2] >= 1 && whitePawnInColumnCount[1] == 0 &&
            whitePawnInColumnCount[3] == 0)
            {
                board.Score -= 16;
            }
            if (whitePawnInColumnCount[3] >= 1 && whitePawnInColumnCount[2] == 0 &&
            whitePawnInColumnCount[4] == 0)
            {
                board.Score -= 20;
            }
            if (whitePawnInColumnCount[4] >= 1 && whitePawnInColumnCount[3] == 0 &&
            whitePawnInColumnCount[5] == 0)
            {
                board.Score -= 20;
            }
            if (whitePawnInColumnCount[5] >= 1 && whitePawnInColumnCount[4] == 0 &&
            whitePawnInColumnCount[6] == 0)
            {
                board.Score -= 16;
            }
            if (whitePawnInColumnCount[6] >= 1 && whitePawnInColumnCount[5] == 0 &&
            whitePawnInColumnCount[7] == 0)
            {
                board.Score -= 14;
            }
            if (whitePawnInColumnCount[7] >= 1 && whitePawnInColumnCount[6] == 0)
            {
                board.Score -= 12;
            }
            //Black Passed Pawns
            if (blackPawnInColumnCount[0] >= 1 && whitePawnInColumnCount[0] == 0)
            {
                board.Score -= blackPawnInColumnCount[0];
            }
            if (blackPawnInColumnCount[1] >= 1 && whitePawnInColumnCount[1] == 0)
            {
                board.Score -= blackPawnInColumnCount[1];
            }
            if (blackPawnInColumnCount[2] >= 1 && whitePawnInColumnCount[2] == 0)
            {
                board.Score -= blackPawnInColumnCount[2];
            }
            if (blackPawnInColumnCount[3] >= 1 && whitePawnInColumnCount[3] == 0)
            {
                board.Score -= blackPawnInColumnCount[3];
            }
            if (blackPawnInColumnCount[4] >= 1 && whitePawnInColumnCount[4] == 0)
            {
                board.Score -= blackPawnInColumnCount[4];
            }
            if (blackPawnInColumnCount[5] >= 1 && whitePawnInColumnCount[5] == 0)
            {
                board.Score -= blackPawnInColumnCount[5];
            }
            if (blackPawnInColumnCount[6] >= 1 && whitePawnInColumnCount[6] == 0)
            {
                board.Score -= blackPawnInColumnCount[6];
            }
            if (blackPawnInColumnCount[7] >= 1 && whitePawnInColumnCount[7] == 0)
            {
                board.Score -= blackPawnInColumnCount[7];
            }
            //White Passed Pawns
            if (whitePawnInColumnCount[0] >= 1 && blackPawnInColumnCount[1] == 0)
            {
                board.Score += whitePawnInColumnCount[0];
            }
            if (whitePawnInColumnCount[1] >= 1 && blackPawnInColumnCount[1] == 0)
            {
                board.Score += whitePawnInColumnCount[1];
            }
            if (whitePawnInColumnCount[2] >= 1 && blackPawnInColumnCount[2] == 0)
            {
                board.Score += whitePawnInColumnCount[2];
            }
            if (whitePawnInColumnCount[3] >= 1 && blackPawnInColumnCount[3] == 0)
            {
                board.Score += whitePawnInColumnCount[3];
            }
            if (whitePawnInColumnCount[4] >= 1 && blackPawnInColumnCount[4] == 0)
            {
                board.Score += whitePawnInColumnCount[4];
            }
            if (whitePawnInColumnCount[5] >= 1 && blackPawnInColumnCount[5] == 0)
            {
                board.Score += whitePawnInColumnCount[5];
            }
            if (whitePawnInColumnCount[6] >= 1 && blackPawnInColumnCount[6] == 0)
            {
                board.Score += whitePawnInColumnCount[6];
            }
            if (whitePawnInColumnCount[7] >= 1 && blackPawnInColumnCount[7] == 0)
            {
                board.Score += whitePawnInColumnCount[7];
            }

            #endregion DoubledIsolatedPawns
        }

        internal MovingPiece SearchForBestMove(Board board, int depth)
        {
            int alpha = -400000000;
            const int beta = 400000000;
            BoardMovePair bestBoardMovePair = default;
            List<BoardMovePair> resultBoards = GetSortedBoardMovePairs(board);

           // Console.WriteLine($"SearchForBestMove. available moves = {resultBoards.Count}" );
            // Searching for instant checkmate
            foreach (var boardMovePair in resultBoards)
            {
                int value = -AlphaBeta(boardMovePair.Board, 1, -beta, -alpha);
                if (value >= 32767)
                {
                    return boardMovePair.Move;
                }
            }
            depth--;
            // search deeper if there are fewer available moves
            depth = (resultBoards.Count <= 15) ? depth + 1 : depth;
            alpha = int.MinValue;
            foreach (var boardMovePair in resultBoards)
            {
                int value = -AlphaBeta(boardMovePair.Board, depth, -beta, -alpha);
                boardMovePair.Board.Score = value;
                //If value is greater then alpha this is the best board
                if (value > alpha)
                {
                    alpha = value;
                    bestBoardMovePair = boardMovePair;
                }
            }
            Console.WriteLine();
            Console.WriteLine($"alphabeta invoked {AlphaBetaInvokeCount} times");
            return bestBoardMovePair.Move;
        }
        /// <summary>
        /// Computes the single piece score.
        /// </summary>
        /// <returns>Score value</returns>
        private int EvaluatePieceScore(Board board, PieceOnSquare piece, bool isEndOfGame, ref bool insufficientMaterial, ref int bishopsCount, ref int bishopsOnWhiteSquareCount, ref int knightsCount)
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
            score += piece.ValidMovesCount;

            switch (piece.Piece)
            {
                case Piece.BlackPawn:
                case Piece.WhitePawn:
                    {
                        insufficientMaterial = false;
                        score += EvaluatePawnScore(piece);
                        break;
                    }
                case Piece.BlackKnight:
                case Piece.WhiteKnight:
                    {
                        knightsCount++;
                        if (knightsCount > 1)
                        {
                            insufficientMaterial = false;
                        }
                        // knights are worth less in the end game since it is difficult to mate with a knight
                        // hence they lose 10 points during the end game.
                        if (isEndOfGame)
                        {
                            score -= 10;
                        }
                        break;
                    }
                case Piece.BlackBishop:
                case Piece.WhiteBishop:
                    {
                        // to check insuffisient material tie rule
                        // king and n * bishop (n > 0) on the same color versus king = draw
                        // king and n * bishop (n > 0) versus king and m * bishops (m > 0) with all bishops on same color = draw
                        if (piece.Square.GetSquareColor() == Moves.Helpers.Color.White)
                        {
                            bishopsOnWhiteSquareCount++;
                        }
                        // Bishops are worth more in the end game, also we add a small bonus for having 2 bishops
                        // since they complement each other by controlling different ranks.
                        bishopsCount++;
                        if (bishopsCount >= 2)
                        {
                            score += 10;
                        }
                        if (isEndOfGame)
                        {
                            score += 10;
                        }
                        break;
                    }
                // Rooks shouldnt leave their corner positions before castling has occured
                case Piece.BlackRook:
                    {
                        insufficientMaterial = false;
                        if (!board.IsBlackCastled && !((piece.Square.X == 0 && board.BlackCastlingFenPart.Contains('k')) || (piece.Square.X == 7 && board.BlackCastlingFenPart.Contains('q'))))
                        {
                            score -= 10;
                        }
                        break;
                    }
                case Piece.WhiteRook:
                    {
                        insufficientMaterial = false;
                        if (!board.IsWhiteCastled && !((piece.Square.X == 0 && board.WhiteCastlingFenPart.Contains('K')) || (piece.Square.X == 7 && board.WhiteCastlingFenPart.Contains('Q'))))
                        {
                            score -= 10;
                        }
                        break;
                    }
                case Piece.BlackQueen:
                case Piece.WhiteQueen:
                    {
                        insufficientMaterial = false;
                        break;
                    }
                case Piece.BlackKing:
                    {
                        // If he has less than 2 move, he possibly one move away from mate.
                        if (piece.ValidMovesCount < 2)
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
                case Piece.WhiteKing:
                    {
                        // If he has less than 2 move, he possibly one move away from mate.
                        if (piece.ValidMovesCount < 2)
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
                case Piece.None:
                default:
                    break;
            }

            // add position value
            score += piece.Piece.GetPieceSquareTableScore(piece.Square.X, piece.Square.Y, isEndOfGame);

            return (piece.Piece.GetColor() == Color.White) ? score : -score;
        }

        /// <summary>
        /// Perform evaluation for pawn: 
        /// Remove some points for pawns on the edge of the board. The idea is that since a pawn of the edge can
        /// only attack one way it is worth 15% less.
        /// Give an extra bonus for pawns that are on the 6th and 7th rank as long as they are not attacked in any way
        /// Add points based on the Pawn Piece Square Table Lookup.
        /// </summary>
        /// <returns></returns>
        private int EvaluatePawnScore(PieceOnSquare piece)
        {
            var score = 0;
            var posX = piece.Square.X;


            if (posX == 0 || posX == 7)
            {
                //Rook Pawns are worth 15% less because they can only attack one way
                score -= 15;
            }

            if (piece.Piece.GetColor() == Moves.Helpers.Color.White)
            {
                if (whitePawnInColumnCount[posX] > 0)
                {
                    //Doubled Pawn
                    score -= 16;
                }
                if (piece.Square.Y == 1)
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

        private List<MovingPiece> EvaluateMoves(Board board)
        {
            var validMoves = new List<MovingPiece>();
            var move = new Move(board);
            MovingPiece currentMovingPiece;
            int destPieceValue, currentMovingPieceValue;
            Piece destPiece;

            foreach (var pieceOnSquare in board.YieldPieces())
            {
                foreach (var squareTo in Square.YieldSquares())
                {
                    currentMovingPiece = new MovingPiece(pieceOnSquare, squareTo);
                    // add score for castling
                    if (currentMovingPiece.IsItCastlingMove())
                    {
                        var targetColor = currentMovingPiece.Piece.GetColor();
                        if (targetColor != board.MoveColor)
                            continue;
                        var isToKingside = currentMovingPiece.SignX > 0;
                        if (board.CanKingCastle(isToKingside))
                        {
                            currentMovingPiece.Score += 40;
                            validMoves.Add(currentMovingPiece);
                        }
                    } // regular move
                    else if (move.CanMove(currentMovingPiece) &&
                        !board.IsCheckAfterMove(currentMovingPiece))
                    {
                        destPiece = board.GetPieceAt(currentMovingPiece.To);
                        if (destPiece != Piece.None)
                        {
                            destPieceValue = destPiece.GetPieceValue();
                            currentMovingPieceValue = currentMovingPiece.Piece.GetPieceValue();
                            currentMovingPiece.Score += destPieceValue;

                            if (currentMovingPieceValue < destPieceValue)
                            {
                                currentMovingPiece.Score += destPieceValue - currentMovingPieceValue;
                            }
                            currentMovingPiece.Score += currentMovingPiece.PieceActionValue;

                            // subtract score for spoiling castling

                            if (board.MoveColor == Color.White && !board.IsWhiteCastled)
                            {
                                if (currentMovingPiece.Piece == Piece.WhiteKing || currentMovingPiece.Piece == Piece.WhiteRook)
                                {
                                    currentMovingPiece.Score -= 40;
                                }
                            }
                            else if (board.MoveColor == Color.Black && !board.IsBlackCastled)
                            {
                                if (currentMovingPiece.Piece == Piece.BlackKing || currentMovingPiece.Piece == Piece.BlackRook)
                                {
                                    currentMovingPiece.Score -= 40;
                                }
                            }
                        }
                        validMoves.Add(currentMovingPiece);
                    }
                }
            }

            return validMoves;
        }

        private int AlphaBeta(Board board, int depth, int alpha, int beta)
        {
            AlphaBetaInvokeCount++;
            if ((board.IsFiftyMovesRuleEnabled && board.FiftyMovesCount >= 50) || (board.IsThreefoldRepetitionRuleEnabled && board.RepeatedMovesCount >= 3))
                return 0;
            if (depth == 0 || board.IsStaleMate)
            {
                EvaluateBoardScore(board);
                return GetScoreAccordingColor(board.Score, board.MoveColor);
            }

            switch (board.MateTo)
            {
                case Color.White:
                    {
                        if(board.MoveColor == Color.Black)
                        {
                            return 32767 + depth;
                        } else
                        {
                            return -32767 - depth;
                        }
                    }
                case Color.Black:
                    {
                        if(board.MoveColor == Color.Black)
                        {
                            return -32767 - depth;
                        } else
                        {
                            return 32767 + depth;
                        }
                    }
                case Color.None:
                default:
                    break;
            }
            if(board.MateTo != Color.None)
            {
                return 32767 + depth;
            }

            var validMoves = new List<MovingPiece>();
            validMoves = EvaluateMoves(board);
            validMoves.Sort();
            Board nextBoard;
            bool isToKingSide;
            foreach (var move in validMoves)
            {
                if(move.IsItCastlingMove())
                {
                    isToKingSide = move.SignX > 0;
                    nextBoard = board.Castle(isToKingSide);
                } else
                {
                    nextBoard = board.Move(move);
                }

                if (nextBoard.IsCheckAfterMove(move))
                {
                    nextBoard.CheckTo = nextBoard.MoveColor;
                    if (!nextBoard.IsMoveAvailable())
                    {
                        nextBoard.MateTo = nextBoard.MoveColor;
                    }
                }
                else if (!nextBoard.IsMoveAvailable())
                {
                    nextBoard.IsStaleMate = true;
                }
                
                var value = -AlphaBeta(nextBoard, depth - 1, -beta, -alpha);
                if (value >= beta)
                {
                    return beta;
                }
                if (value > alpha)
                {
                    alpha = value;
                }
            }
            return alpha;
            
        }

        private int GetScoreAccordingColor(int score, Color color)
        {
            return (color == Color.Black) ? -score : score;
        }

        private List<BoardMovePair> GetSortedBoardMovePairs(Board board)
        {
            var result = new List<BoardMovePair>();
            var currentMove = new Move(board);
            Board nextBoard;
            MovingPiece movingPiece;
            foreach (var pieceOnSquare in board.YieldPieces())
            {
                foreach (var squareTo in Square.YieldSquares())
                {
                    movingPiece = new MovingPiece(pieceOnSquare, squareTo);
                    if (currentMove.CanMove(movingPiece) &&
                        !board.IsCheckAfterMove(movingPiece))
                    {
                        nextBoard = board.Move(movingPiece);
                        EvaluateBoardScore(nextBoard);
                        nextBoard.Score = GetScoreAccordingColor(nextBoard.Score, nextBoard.MoveColor);
                        result.Add(new BoardMovePair(nextBoard, movingPiece));
                    }
                    else if (movingPiece.IsItCastlingMove())
                    {
                        var targetColor = movingPiece.Piece.GetColor();
                        if (targetColor != board.MoveColor)
                            continue;
                        var isToKingside = movingPiece.SignX > 0;
                        if (board.CanKingCastle(isToKingside))
                        {
                            nextBoard = board.Castle(isToKingside);
                            EvaluateBoardScore(nextBoard);
                            nextBoard.Score = GetScoreAccordingColor(nextBoard.Score, nextBoard.MoveColor);
                            result.Add(new BoardMovePair(nextBoard, movingPiece));
                        }
                    }
                }
            }
            result.Sort();

            return result;
        }

        private struct BoardMovePair : IComparable<BoardMovePair>
        {
            public Board Board { get; set; }
            public MovingPiece Move { get; set; }
            public BoardMovePair(Board board, MovingPiece move)
            {
                Board = board;
                Move = move;
            }

            public int CompareTo(BoardMovePair other)
            {
                return other.Board.Score.CompareTo(Board.Score);
            }
        }
    }
}


