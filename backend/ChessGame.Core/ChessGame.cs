using Chess.BL.Figures;
using Chess.BL.Figures.Helpers;
using Chess.BL.Moves;
using Chess.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.BL
{
    public class ChessGame : IChessGame
    {
        private Board board;
        private Move currentMove;
        public const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string Fen { get; private set; }

#warning добавить инфу кто кому что сделал
        public static event EventHandler Check;
        public static event EventHandler Mate;
        /// <summary>
        /// Forsyth–Edwards Notation (FEN) is a standard notation for describing a particular board position of a chess game. The purpose of FEN is to provide all the necessary information to restart a game from a particular position.
        /// </summary>
        /// <param name="fen">Forsyth–Edwards Notation</param>
        /// <remarks>https://en.wikipedia.org/wiki/Forsyth–Edwards_Notation</remarks>
        public ChessGame()
        {
        }

        private ChessGame(Board board)
        {
            this.board = board;
            Fen = board.Fen;
            currentMove = new Move(board);
        }

        public IChessGame InitGame(string fen = DefaultFen)
        {
            //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            // 0-позиция фигур                             1 2    3 4 5
            // 0 - позиция фигур,  1 - чей ход, 2 - флаги рокировки
            // 3 - правило битого поля, 4 - колич. ходов для правила 50 ходов
            // 5 - номер хода
            Fen = fen;
            board = new Board(fen);
            currentMove = new Move(board);
            return this;
        }
        public IChessGame Move(string move) // Pe2e4  Pe7e8Q k0-0-0
        {
            if(move.Contains('0'))
            {
                return Castle(move);
            }

            var movingFigure = new MovingFigure(move);
            if (!currentMove.CanMove(movingFigure))
                return this;
            if (board.IsCheckAfterMove(movingFigure))
            {
                return this;
            }
            var nextBoard = board.Move(movingFigure);
            var nextChessPosition = new ChessGame(nextBoard);

            if (nextBoard.IsCheckAfterMove(movingFigure))
            {
                Check?.Invoke(nextChessPosition, null);
                if (nextChessPosition.ComputeAllMoves().Count < 1)
                    Mate?.Invoke(nextChessPosition, null);
            }
            return nextChessPosition;
        }

        public IChessGame Castle(string move)
        {
            var king = (Figure)move[0];
            var rookFigure = (king == Figure.BlackKing) ? Figure.BlackRook : Figure.WhiteRook;
            var y = (king == Figure.BlackKing) ? 7 : 0;
            var stepX = (move.Split('-').Length == 3) ? -1 : 1;
            if (!CanKingCastle(stepX > 0, king.GetColor()))
            {
                return this;
            }
            MovingFigure mf;
            FigureOnSquare rook;

            if(stepX == -1) // additional check required for rook
            {
                rook = new FigureOnSquare(rookFigure, new Square(0, y));
                mf = new MovingFigure(rook, new Square(1, y));
                if (!currentMove.CanMove(mf))
                    return this;
            } else
            {
                rook = new FigureOnSquare(rookFigure, new Square(7, y));
            }
            var firstKingDestSquare = new Square(4 + stepX, y);
            mf = new MovingFigure(new FigureOnSquare(king, new Square(4, y)), firstKingDestSquare);
            if (!currentMove.CanMove(mf))
                return this;
            if (board.IsCheckAfterMove(mf))
                return this;

            var finalKingDestSquare = new Square(firstKingDestSquare.X + stepX, y);
            mf = new MovingFigure(new FigureOnSquare(king, firstKingDestSquare), finalKingDestSquare);
            if (!currentMove.CanMove(mf))
                return this;
            if (board.IsCheckAfterMove(mf))
                return this;

            var nextBoard = board.Castle(new MovingFigure(new FigureOnSquare(king, new Square(4, y)), finalKingDestSquare), new MovingFigure(rook, firstKingDestSquare));
            return new ChessGame(nextBoard);
        }

        public char GetFigureAt(int x, int y)
        {
            var targetSquare = new Square(x, y);
            var figure = board.GetFigureAt(targetSquare);
            return figure == Figure.None ? '.' : (char)figure;
        }

        public List<string> GetAllValidMovesForFigureAt(int x, int y)
        {
            var validMoves = new List<string>();
            var targetSquare = new Square(x, y);
            if (!targetSquare.IsOnBoard())
                return validMoves;

            var targetFigure = board.GetFigureAt(targetSquare);
            if (targetFigure == Figure.None)
                return validMoves;

            var figureOnSquare = new FigureOnSquare(targetFigure, targetSquare);
            MovingFigure movingFigure;
            foreach (var squareTo in Square.YieldSquares())
            {
                movingFigure = new MovingFigure(figureOnSquare, squareTo);
                if (currentMove.CanMove(movingFigure) &&
                    !board.IsCheckAfterMove(movingFigure))
                    validMoves.Add(((char)('a' + squareTo.X)).ToString() + (squareTo.Y + 1));
            }

            return validMoves;
        }

#warning перенести в board
        private List<MovingFigure> ComputeAllMoves()
        {
            var allMoves = new List<MovingFigure>();
            MovingFigure movingFigure;
            foreach (var figureOnSquare in board.YieldFigures())
            {
                foreach (var squareTo in Square.YieldSquares())
                {
                    movingFigure = new MovingFigure(figureOnSquare, squareTo);
                    if (currentMove.CanMove(movingFigure) &&
                        !board.IsCheckAfterMove(movingFigure))
                        allMoves.Add(movingFigure);
                }
            }

            return allMoves;
        }
    
        private bool CanKingCastle(bool isKingside, Moves.Helpers.Color color)
        {
            var currentCastrlingFenPart = ((color == Moves.Helpers.Color.White) ? board.WhiteCastlingFenPart : board.BlackCastlingFenPart).ToLower();
            return (isKingside) ? currentCastrlingFenPart.Contains('k') : currentCastrlingFenPart.Contains('q');
        }

        public bool Equals(IChessGame other)
        {
            if (other == null || !string.Equals(this.Fen, other.Fen))
                return false;
            else
                return true;
        }
    }
}
