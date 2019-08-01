using ChessGame.Core.Figures;
using ChessGame.Core.Figures.Helpers;

namespace ChessGame.Core.Moves
{
    internal sealed class Move
    {
        private MovingFigure movingFigure;
        private Board board;

        public Move(Board board)
        {
            this.board = board;
        }

        public bool CanMove(MovingFigure movingFigure)
        {
            this.movingFigure = movingFigure;
            return movingFigure.To.IsOnBoard() && CanFigureMove() && CanMove() && CanMoveTo();
        }

        private bool CanMove()
        {
            return movingFigure.Figure.GetColor() == board.MoveColor;
        }

        private bool CanMoveTo()
        {
            return movingFigure.From != movingFigure.To &&
                board.GetFigureAt(movingFigure.To).GetColor() != movingFigure.Figure.GetColor();
        }

        private bool CanFigureMove()
        {
            switch (movingFigure.Figure)
            {
                case Figure.None:
                    break;
                case Figure.WhiteKing:
                case Figure.BlackKing:
                    {
                        return movingFigure.AbsDeltaX <= 1 && movingFigure.AbsDeltaY <= 1;
                    }
                case Figure.WhiteQueen:
                case Figure.BlackQueen:
                    {
                        return CanStraightMove();
                    }
                case Figure.WhiteRook:
                case Figure.BlackRook:
                    {
                        return (movingFigure.SignX == 0 || movingFigure.SignY == 0) &&
                            CanStraightMove();
                    }
                case Figure.WhiteBishop:
                case Figure.BlackBishop:
                    {
                        return (movingFigure.SignX != 0 && movingFigure.SignY != 0) &&
                            CanStraightMove();
                    }
                case Figure.WhiteKnight:
                case Figure.BlackKnight:
                    {
                        return (movingFigure.AbsDeltaX == 1 && movingFigure.AbsDeltaY == 2) ||
                          (movingFigure.AbsDeltaX == 2 && movingFigure.AbsDeltaY == 1);
                    }
                case Figure.WhitePawn:
                case Figure.BlackPawn:
                    {
                        return CanPawnMove();
                    }
                default:
                    break;
            }
            return false;
        }
        
        private bool CanPawnMove()
        {
            int stepY = movingFigure.Figure.GetColor() == Helpers.Color.White ? 1 : -1;
            return
                CanPawnGo() ||
                CanPawnJump() ||
                CanPawnAttack();

            bool CanPawnGo()
            {
                if (board.GetFigureAt(movingFigure.To) == Figure.None)
                {
                    if (movingFigure.DeltaX == 0 && movingFigure.DeltaY == stepY)
                    {
                        return true;
                    }
                }

                return false;
            }

            bool CanPawnJump()
            {
                if (movingFigure.From.Y == 1 || movingFigure.From.Y == 6)
                {
                    if (board.GetFigureAt(movingFigure.To) == Figure.None)
                    {
                        if (movingFigure.DeltaX == 0
                            && movingFigure.DeltaY == 2 * stepY)
                        {
                            if (board.GetFigureAt(movingFigure.From.X, movingFigure.From.Y + stepY) == Figure.None)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            bool CanPawnAttack()
            {
                if (board.GetFigureAt(movingFigure.To) != Figure.None)
                {
                    if (movingFigure.AbsDeltaX == 1 &&
                        movingFigure.DeltaY == stepY)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private bool CanStraightMove()
        {
            var currentSquare = movingFigure.From;
            do
            {
                currentSquare = new Square(currentSquare.X + movingFigure.SignX, currentSquare.Y + movingFigure.SignY);
                if (currentSquare == movingFigure.To)
                    return true;
            } while (currentSquare.IsOnBoard() && board.GetFigureAt(currentSquare) == Figure.None);
            return false;
        }
    }
}
