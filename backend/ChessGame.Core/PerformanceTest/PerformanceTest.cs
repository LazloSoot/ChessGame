using ChessGame.Core.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ChessGame.Core.PerformanceTest
{
    internal static class PerformanceTest
    {
        internal static void Run(Board board, int depth)
        {
            var stopWatch = new Stopwatch();
            int checkedNodesCount, quiescenceCheckedNodesCount;
            checkedNodesCount = quiescenceCheckedNodesCount = 0;
            stopWatch.Start();
            var nodes = Search(board, depth);
            stopWatch.Stop();
            Debug.WriteLine($"Depth: {depth}");
            Debug.WriteLine($"Nodes count: {nodes}");
            Debug.WriteLine($"Elapsed ms: {stopWatch.ElapsedMilliseconds}");
        }

        internal static long Search(Board board, int depth)
        {
            long nodes = 0;

            if (depth == 0) return 1;

            var boardMovePairs = Evaluation.BoardEvaluation.GetSortedBoardMovePairs(board);

            foreach (var boardMovePair in boardMovePairs)
            {
                nodes += Search(boardMovePair.Board, depth - 1);
            }

            return nodes;
        }

    }
}

