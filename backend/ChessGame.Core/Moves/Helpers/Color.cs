using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChessGame.Test")]
namespace ChessGame.Core.Moves.Helpers
{
    internal enum Color
    {
        None,
        White = 'w',
        Black = 'b'
    }
}
