using Chess.BL.Figures.Helpers;
using Chess.BL.Moves;
using Chess.BL.Moves.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Chess.BL.Figures
{
    class Board
    {
        private Figure[,] figures;

        public string Fen { get; private set; }
        public Color MoveColor { get; set; }
        public int MoveNumber { get; set; }

        public Board(string fen, int moveNumber = 0)
        {
            Fen = fen;
            figures = new Figure[8, 8];
            MoveNumber = moveNumber < 0 ? 0 : moveNumber;
            InitFiguresPosition();
        }

        public Figure GetFigureAt(Square square)
        {
            return figures[square.X, square.Y];
        }

        public Figure GetFigureAt(int x, int y)
        {
            return figures[x, y];
        }

        public Board Move(MovingFigure mf)
        {
            var nextBoardState = new Board(Fen);
            nextBoardState.SetFigureAt(mf.From, Figure.None);
            nextBoardState.SetFigureAt(mf.To, mf.Promotion == Figure.None ? mf.Figure : mf.Promotion);

            if (MoveColor == Color.Black)
                nextBoardState.MoveNumber = this.MoveNumber + 1;

            nextBoardState.MoveColor = MoveColor.FlipColor();
            nextBoardState.GenerateNextFen();
            return nextBoardState;

        }

        public IEnumerable<FigureOnSquare> YieldFigures()
        {
            foreach (var s in Square.YieldSquares())
            {
                if (GetFigureAt(s).GetColor() == MoveColor)
                    yield return new FigureOnSquare(GetFigureAt(s), s);
            }
        }
        public bool IsCheckAfterMove(MovingFigure movingFigure)
        {
            var after = Move(movingFigure);
            return after.IsCheckTo();
        }

        public bool IsCheckTo()
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

            this.Fen = figuresBldr.Append($" {(char)MoveColor} - - 0 {MoveNumber}").ToString();
        }

        private void InitFiguresPosition()
        {
            //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            // 0-позиция фигур                             1 2    3 4 5
            // 0 - позиция фигур,  1 - чей ход, 2 - флаги рокировки
            // 3 - правило битого поля, 4 - колич. ходов для правила 50 ходов
            // 5 - номер хода

            string[] parts = Fen.Split();
            if (parts.Length < 6)
                return;
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
