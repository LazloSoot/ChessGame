using Chess.Common.Helpers;
using Chess.Common.Interfaces;
using ChessGame.Core.Pieces;
using ChessGame.Core.Pieces.Helpers;
using ChessGame.Core.Moves;
using ChessGame.Core.Moves.Helpers;
using System.Collections.Generic;
using System;
using ChessGame.Core.Evaluation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChessGame.Core
{
    public class ChessGameEngine : IChessGame
    {
        private Move _currentMove;
        public const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string Fen { get; private set; }
        public Chess.Common.Helpers.ChessGame.Color MateTo { get => (Chess.Common.Helpers.ChessGame.Color)Board.MateTo; private set => Board.MateTo = (Color)value; }
        public Chess.Common.Helpers.ChessGame.Color CheckTo { get => (Chess.Common.Helpers.ChessGame.Color)Board.CheckTo; private set => Board.CheckTo = (Color)value; }
        public bool IsStaleMate { get => Board.IsStaleMate; private set => Board.IsStaleMate = value; }
        public bool IsInsufficientMaterial { get => Board.IsInsufficientMaterial; private set => Board.IsInsufficientMaterial = value; }
        internal Board Board { get; private set; }
        // Defined by the amount of pieces remaining on the board in the evaluation function.If the chess board is in an end game
        // state certain behaviors will be modified to increase king safety and mate opportunities.
        internal bool IsEndOfGamePhase { get => Board.IsEndOfGamePhase; private set => Board.IsEndOfGamePhase = value; }
        /// <summary>
        /// Forsyth–Edwards Notation (FEN) is a standard notation for describing a particular board position of a chess game. The purpose of FEN is to provide all the necessary information to restart a game from a particular position.
        /// </summary>
        /// <param name="fen">Forsyth–Edwards Notation</param>
        /// <remarks>https://en.wikipedia.org/wiki/Forsyth–Edwards_Notation</remarks>
        public ChessGameEngine()
        {
        }

        private ChessGameEngine(Board board)
        {
            Board = board;
            Fen = board.Fen;
            _currentMove = new Move(board);
        }

        public IChessGame InitGame(string fen = DefaultFen)
        {
            //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            // 0-позиция фигур                             1 2    3 4 5
            // 0 - позиция фигур,  1 - чей ход, 2 - флаги рокировки
            // 3 - правило битого поля, 4 - колич. ходов для правила 50 ходов
            // 5 - номер хода
            Fen = fen;
            Board = new Board(fen);
            _currentMove = new Move(Board);
            return this;
        }

        public IChessGame InitGame(Chess.Common.Helpers.ChessGame.ChessGameInitSettings initialSettings)
        {
            Fen = initialSettings.Fen;
            Board = new Board(Fen)
            {
                IsBlackCastled = initialSettings.IsBlackCastled,
                IsWhiteCastled = initialSettings.IsWhiteCastled,
                IsEnpassantRuleEnabled = initialSettings.IsEnpassantRuleEnabled,
                IsFiftyMovesRuleEnabled = initialSettings.IsFiftyMovesRuleEnabled,
                IsThreefoldRepetitionRuleEnabled = initialSettings.IsThreefoldRepetitionRuleEnabled
            };
            _currentMove = new Move(Board);
            return this;
        }

        public IChessGame Move(string move) // Pe2e4  Pe7e8Q
        {
            
            var movingPiece = new MovingPiece(move);
            Board nextBoard;
            if (Board.GetPieceAt(movingPiece.From) == Piece.None)
                return this;
            if (movingPiece.IsItCastlingMove()) // its castling
            {
                var targetColor = movingPiece.Piece.GetColor();
                if (targetColor != Board.MoveColor)
                    return this;
                var isToKingside = movingPiece.SignX > 0;
                if (Board.CanKingCastle(isToKingside))
                {
                    nextBoard = Board.Castle(isToKingside);
                }
                else
                {
                    return this;
                }
            }
            else if (!_currentMove.CanMove(movingPiece) || Board.IsIGotCheckAfterMove(movingPiece))
            {
                return this;
            } else
            {
                nextBoard = Board.Move(movingPiece);
            }
            
            var nextChessPosition = new ChessGameEngine(nextBoard);

            if(nextBoard.IsIGotCheckAfterMove(movingPiece))
            {
                nextChessPosition.CheckTo = (Chess.Common.Helpers.ChessGame.Color)nextBoard.MoveColor;
                if(!nextBoard.IsMoveAvailable())
                {
                    nextChessPosition.MateTo = (Chess.Common.Helpers.ChessGame.Color)nextBoard.MoveColor;
                }
            } else if(!nextBoard.IsMoveAvailable())
            {
                nextChessPosition.IsStaleMate = true;
            }

            return nextChessPosition;
        }
        
        public IChessGame ComputerMove()
        {
            int checkedNodesCount, quiescenceNodesCount;
            checkedNodesCount = quiescenceNodesCount = 0;
            var bestMove = BoardEvaluation.SearchForBestMove(Board, 3, ref checkedNodesCount, ref quiescenceNodesCount);
            Board nextBoard;
            if (bestMove.IsItCastlingMove()) // its castling
            {
                var isToKingside = bestMove.SignX > 0;
                nextBoard = Board.Castle(isToKingside);
            } else
            {
                nextBoard = Board.Move(bestMove);
            }

            nextBoard.CheckBoard();
            Debug.WriteLine($"Total nodes checked: {checkedNodesCount + quiescenceNodesCount}");
            Debug.WriteLine($"Quiescence nodes checked: {quiescenceNodesCount}");
            return new ChessGameEngine(nextBoard);
        }

        public char GetPieceAt(int x, int y)
        {
            var targetSquare = new Square(x, y);
            var piece = Board.GetPieceAt(targetSquare);
            return (char)piece;
        }

        public List<string> GetAllValidMovesForPieceAt(int x, int y)
        {
            var validMoves = new List<string>();
            var targetSquare = new Square(x, y);
            if (!targetSquare.IsOnBoard())
                return validMoves;

            var targetPiece = Board.GetPieceAt(targetSquare);
            if (targetPiece == Piece.None || targetPiece.GetColor() != Board.MoveColor)
                return validMoves;

            var pieceOnSquare = new PieceOnSquare(targetPiece, targetSquare);
            MovingPiece movingPiece;
            foreach (var squareTo in Square.YieldSquares())
            {
                movingPiece = new MovingPiece(pieceOnSquare, squareTo);
                if (_currentMove.CanMove(movingPiece) &&
                    !Board.IsIGotCheckAfterMove(movingPiece))
                    validMoves.Add(((char)('a' + squareTo.X)).ToString() + (squareTo.Y + 1));
            }

            if(targetPiece == Piece.BlackKing || targetPiece == Piece.WhiteKing)
            {
                if (Board.CanKingCastle(isToKingside: true))
                {
                    validMoves.Add($"g{y + 1}");
                }
                if (Board.CanKingCastle(isToKingside: false))
                {
                    validMoves.Add($"c{y + 1}");
                }
            }

            return validMoves;
        }
        
        public bool Equals(IChessGame other)
        {
            if (other == null || !string.Equals(this.Fen, other.Fen))
                return false;
            else
                return true;
        }

        public void RunPerfTest(int depth)
        {
            Task.Run(() =>
            {
                for (int i = 0; i <= depth; i++)
                {
                    PerformanceTest.PerformanceTest.Run(new Board(DefaultFen), i);
                }
            });
           
        }
    }
}
