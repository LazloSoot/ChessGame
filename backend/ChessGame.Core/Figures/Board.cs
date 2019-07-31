using Chess.BL.Figures.Helpers;
using Chess.BL.Moves;
using Chess.BL.Moves.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.BL.Figures
{
    internal sealed class Board
    {
        private Figure[,] figures;

        internal string Fen { get; private set; }
        internal string WhiteCastlingFenPart { get; set; }
        internal string BlackCastlingFenPart { get; set; }
        internal Color MoveColor { get; set; }
        internal int MoveNumber { get; set; }

        internal Board(string fen, int moveNumber = 0)
        {
            Fen = fen;
            figures = new Figure[8, 8];
            MoveNumber = moveNumber < 0 ? 0 : moveNumber;
            InitFiguresPosition();
        }

        internal Figure GetFigureAt(Square square)
        {
            return figures[square.X, square.Y];
        }

        internal Figure GetFigureAt(int x, int y)
        {
            return figures[x, y];
        }

        internal Board Move(MovingFigure mf)
        {
            var nextBoardState = new Board(Fen);
            
            nextBoardState.SetFigureAt(mf.From, Figure.None);
            nextBoardState.SetFigureAt(mf.To, mf.Promotion == Figure.None ? mf.Figure : mf.Promotion);

            if (MoveColor == Color.Black)
                nextBoardState.MoveNumber = MoveNumber + 1;

            nextBoardState.MoveColor = MoveColor.FlipColor();
            nextBoardState.UpdateCastlingData(mf);
            nextBoardState.GenerateNextFen();
            return nextBoardState;
        }

        internal Board Castle(MovingFigure king, MovingFigure rook)
        {
            var nextBoardState = new Board(Fen);

            nextBoardState.SetFigureAt(king.From, Figure.None);
            nextBoardState.SetFigureAt(king.To, king.Figure);
            nextBoardState.SetFigureAt(rook.From, Figure.None);
            nextBoardState.SetFigureAt(rook.To, rook.Figure);

            if (MoveColor == Color.Black)
                nextBoardState.MoveNumber = MoveNumber + 1;

            nextBoardState.MoveColor = MoveColor.FlipColor();
            nextBoardState.UpdateCastlingData(king);
            nextBoardState.GenerateNextFen();
            return nextBoardState;
        }

        private void UpdateCastlingData(MovingFigure mf)
        {
            var targetColor = mf.Figure.GetColor();
            var currentCastlingFenPart = (targetColor == Color.White) ? WhiteCastlingFenPart : BlackCastlingFenPart;
            if (string.IsNullOrWhiteSpace(currentCastlingFenPart))
                return;

            switch (mf.Figure)
            {
                case (Figure.BlackRook):
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
                case (Figure.WhiteRook):
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
                case (Figure.WhiteKing):
                    {
                        WhiteCastlingFenPart = string.Empty;
                        break;
                    }
                case (Figure.BlackKing):
                    {
                        BlackCastlingFenPart = string.Empty;
                        break;
                    }
            }
        }

        internal Board GetBoardAfterFirstKingCastlingMove(MovingFigure king)
        {
            var nextBoardState = new Board(Fen);
            nextBoardState.SetFigureAt(king.From, Figure.None);
            nextBoardState.SetFigureAt(king.To, king.Figure);
            nextBoardState.GenerateNextFen();
            return nextBoardState;
        }

        internal IEnumerable<FigureOnSquare> YieldFigures()
        {
            foreach (var s in Square.YieldSquares())
            {
                if (GetFigureAt(s).GetColor() == MoveColor)
                    yield return new FigureOnSquare(GetFigureAt(s), s);
            }
        }

        internal bool IsCheckAfterMove(MovingFigure movingFigure)
        {
            var after = Move(movingFigure);
            return after.IsCheckTo();
        }

        internal bool IsCheckTo()
        {
            Square targetKingPosition = FindTargetKingPosition();
            var move = new Move(this);
            MovingFigure currentMovingFigure;
            foreach (var figureOnSquare in YieldFigures())
            {
                currentMovingFigure = new MovingFigure(figureOnSquare, targetKingPosition);
                if (move.CanMove(currentMovingFigure))
                    return true;
            }

            return false;
        }

        private Square FindTargetKingPosition()
        {
            var targetFigure = MoveColor == Color.Black ? Figure.WhiteKing : Figure.BlackKing;
            foreach (var square in Square.YieldSquares())
            {
                if (GetFigureAt(square) == targetFigure)
                    return square;
            }
            return default(Square);
        }

        private void GenerateNextFen()
        {

            var figuresBldr = new StringBuilder();
            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                {
                    figuresBldr.Append(figures[x, y] == Figure.None ? '1' : (char)figures[x, y]);
                }
                if (y > 0)
                    figuresBldr.Append('/');
            }
            var eight = "11111111";
            for (int i = 8; i >= 2; i--)
            {
                figuresBldr.Replace(eight.Substring(0, i), i.ToString());
            }
            //  "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            var castlingFenPart = (string.IsNullOrEmpty(WhiteCastlingFenPart) && string.IsNullOrEmpty(BlackCastlingFenPart)) ? "-" : $"{WhiteCastlingFenPart}{BlackCastlingFenPart}";
            Fen = figuresBldr.Append($" {(char)MoveColor} {castlingFenPart} - 0 {MoveNumber}").ToString();
        }

        private void InitFiguresPosition()
        {
            string[] parts = Fen.Split();
            if (parts.Length < 6)
                return;
            var castlingFenPart = parts[2];
            WhiteCastlingFenPart = new string(castlingFenPart.Where(c => char.IsUpper(c)).ToArray());
            BlackCastlingFenPart = new string(castlingFenPart.Where(c => char.IsLower(c)).ToArray());

            InitFigures(parts[0]);
            MoveColor = string.Equals("b", parts[1].Trim().ToLower()) ? Color.Black : Color.White;
            MoveNumber = int.Parse(parts[5]);

            void InitFigures(string data)
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
                        figures[x, y] = (Figure)lines[7 - y][x];
                    }
                }
            }
        }

        private void SetFigureAt(Square square, Figure figure)
        {
            figures[square.X, square.Y] = figure;

        }
    }
}
