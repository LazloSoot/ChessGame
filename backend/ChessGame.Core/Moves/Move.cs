using ChessGame.Core.Pieces;
using ChessGame.Core.Pieces.Helpers;

namespace ChessGame.Core.Moves
{
    internal sealed class Move
    {
        private MovingPiece _movingPiece;
        private Board board;

        public Move(Board board)
        {
            this.board = board;
        }

        public bool CanMove(MovingPiece movingPiece)
        {
            _movingPiece = movingPiece;
            return movingPiece.To.IsOnBoard() && CanPieceMove() && CanMove() && CanMoveTo();
        }

        /// <summary>
        /// Cheks if move is valid, initialize attackedValue/defendedValue of captured piece.
        /// </summary>
        /// <param name="movingPiece">Piece that tries to commit certain move.</param>
        /// <param name="capturedPiece">Piece on destanation square with initialized attackedValue/defendedValue properties.</param>
        /// <returns>Move validation result</returns>
        public bool CanMove(MovingPiece movingPiece, out PieceOnSquare capturedPiece)
        {
            _movingPiece = movingPiece;
            if(movingPiece.To.IsOnBoard() && CanPieceMove() && CanMove() && _movingPiece.From != _movingPiece.To)
            {
                var piece = board.GetPieceAt(_movingPiece.To);
                if(piece != Piece.None)
                {
                    capturedPiece = new PieceOnSquare(piece, movingPiece.To);
                    if(piece.GetColor() != _movingPiece.Piece.GetColor())
                    {
                        capturedPiece.AttackedValue += movingPiece.PieceActionValue;
                        return true;
                    }
                    else
                    {
                        capturedPiece.DefendedValue += movingPiece.PieceActionValue;
                        return false;
                    }
                } 
                else
                {
                    capturedPiece = null;
                    return true;
                }
            }

            capturedPiece = null;
            return false;
        }

        private bool CanMove()
        {
            return _movingPiece.Piece.GetColor() == board.MoveColor;
        }

        private bool CanMoveTo()
        {
            return _movingPiece.From != _movingPiece.To &&
                board.GetPieceAt(_movingPiece.To).GetColor() != _movingPiece.Piece.GetColor();
        }

        private bool CanPieceMove()
        {
            switch (_movingPiece.Piece)
            {
                case Piece.None:
                    break;
                case Piece.WhiteKing:
                case Piece.BlackKing:
                    {
                        return _movingPiece.AbsDeltaX <= 1 && _movingPiece.AbsDeltaY <= 1;
                    }
                case Piece.WhiteQueen:
                case Piece.BlackQueen:
                    {
                        return CanStraightMove();
                    }
                case Piece.WhiteRook:
                case Piece.BlackRook:
                    {
                        return (_movingPiece.SignX == 0 || _movingPiece.SignY == 0) &&
                            CanStraightMove();
                    }
                case Piece.WhiteBishop:
                case Piece.BlackBishop:
                    {
                        return (_movingPiece.SignX != 0 && _movingPiece.SignY != 0) &&
                            CanStraightMove();
                    }
                case Piece.WhiteKnight:
                case Piece.BlackKnight:
                    {
                        return (_movingPiece.AbsDeltaX == 1 && _movingPiece.AbsDeltaY == 2) ||
                          (_movingPiece.AbsDeltaX == 2 && _movingPiece.AbsDeltaY == 1);
                    }
                case Piece.WhitePawn:
                case Piece.BlackPawn:
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
            int stepY = _movingPiece.Piece.GetColor() == Helpers.Color.White ? 1 : -1;
            return
                CanPawnGo() ||
                CanPawnJump() ||
                CanPawnAttack();

            bool CanPawnGo()
            {
                if (board.GetPieceAt(_movingPiece.To) == Piece.None)
                {
                    if (_movingPiece.DeltaX == 0 && _movingPiece.DeltaY == stepY)
                    {
                        return true;
                    }
                }

                return false;
            }

            bool CanPawnJump()
            {
                if (_movingPiece.From.Y == 1 || _movingPiece.From.Y == 6)
                {
                    if (board.GetPieceAt(_movingPiece.To) == Piece.None)
                    {
                        if (_movingPiece.DeltaX == 0
                            && _movingPiece.DeltaY == 2 * stepY)
                        {
                            if (board.GetPieceAt(_movingPiece.From.X, _movingPiece.From.Y + stepY) == Piece.None)
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
                if (board.GetPieceAt(_movingPiece.To) != Piece.None)
                {
                    if (_movingPiece.AbsDeltaX == 1 &&
                        _movingPiece.DeltaY == stepY)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private bool CanStraightMove()
        {
            var currentSquare = _movingPiece.From;
            do
            {
                currentSquare = new Square(currentSquare.X + _movingPiece.SignX, currentSquare.Y + _movingPiece.SignY);
                if (currentSquare == _movingPiece.To)
                    return true;
            } while (currentSquare.IsOnBoard() && board.GetPieceAt(currentSquare) == Piece.None);
            return false;
        }
    }
}
