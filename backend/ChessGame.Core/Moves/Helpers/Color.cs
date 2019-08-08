using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("ChessGame.Test")]
#endif
namespace ChessGame.Core.Moves.Helpers
{
    internal enum Color
    {
        None,
        White = 'w',
        Black = 'b'
    }
}
