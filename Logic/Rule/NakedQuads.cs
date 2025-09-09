using SudokuStepByStep.Common;
using SudokuStepByStep.Models;
using System.Collections.Generic;
using System.Linq;

namespace SudokuStepByStep.Logic.Rule
{
    public static class NakedQuads
    {
        public static SolveStep Run(SudokuSquare[,] squares)
        {
            bool NakedQuadsFound = false;

            var solveStep = new SolveStep()
            {
                Rule = Enums.SolvingRule.NakedQuads,
                HighlightedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedNumbers = new List<int>()
            };

            // Helper to process a group of 4 cells
            void CheckGroup(List<(int row, int col)> cells, List<(int row, int col)> allCells, string groupType)
            {
                var quadCandidates = new HashSet<int>();

                foreach (var cell in cells)
                {
                    quadCandidates.UnionWith(squares[cell.row, cell.col].PossibleNumbers);
                }

                if (quadCandidates.Count == 4)
                {
                    // Check that each cell only contains candidates from quadCandidates
                    if (cells.All(cell => squares[cell.row, cell.col].PossibleNumbers.All(n => quadCandidates.Contains(n))))
                    {
                        // Remove quadCandidates from other cells in the group
                        foreach (var cell in cells)
                        {
                            solveStep.HighlightedSquares.Add(cell);
                        }

                        foreach (var cell in allCells.Except(cells))
                        {
                            foreach (var number in quadCandidates)
                            {
                                if (squares[cell.row, cell.col].PossibleNumbers.Contains(number))
                                {
                                    NakedQuadsFound = true;
                                    solveStep.CandidatesRemovedSquares.Add(cell);

                                    if (!solveStep.CandidatesRemovedNumbers.Contains(number))
                                    {
                                        solveStep.CandidatesRemovedNumbers.Add(number);
                                    }
                                }
                            }
                        }

                        if (solveStep.CandidatesRemovedSquares.Count > 0)
                        {
                            solveStep.Explanation = $"Naked Quad: Numbers {string.Join(", ", quadCandidates)} only appear in four squares in the same {groupType}.{Environment.NewLine}Remove these numbers from other squares in the {groupType}.";
                        }
                    }
                }
            }

            // Check rows
            for (int row = 0; row < 9; row++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(col => squares[row, col].PossibleNumbers.Count >= 2 && squares[row, col].PossibleNumbers.Count <= 4)
                    .Select(col => (row, col)).ToList();

                if (cells.Count >= 4)
                {
                    foreach (var quad in cells.Combinations(4))
                    {
                        CheckGroup(quad.ToList(), Enumerable.Range(0, 9).Select(col => (row, col)).ToList(), "row");

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                        {
                            return solveStep;
                        }
                    }
                }
            }

            // Check columns
            for (int col = 0; col < 9; col++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(row => squares[row, col].PossibleNumbers.Count >= 2 && squares[row, col].PossibleNumbers.Count <= 4)
                    .Select(row => (row, col)).ToList();

                if (cells.Count >= 4)
                {
                    foreach (var quad in cells.Combinations(4))
                    {
                        CheckGroup(quad.ToList(), Enumerable.Range(0, 9).Select(row => (row, col)).ToList(), "column");

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                        {
                            return solveStep;
                        }
                    }
                }
            }

            // Check boxes
            for (int boxRow = 0; boxRow < 3; boxRow++)
            {
                for (int boxCol = 0; boxCol < 3; boxCol++)
                {
                    var boxCells = new List<(int row, int col)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (squares[r, c].PossibleNumbers.Count >= 2 && squares[r, c].PossibleNumbers.Count <= 4)
                            {
                                boxCells.Add((r, c));
                            }
                        }
                    }

                    if (boxCells.Count >= 4)
                    {
                        foreach (var quad in boxCells.Combinations(4))
                        {
                            CheckGroup(quad.ToList(),
                                Enumerable.Range(boxRow * 3, 3)
                                    .SelectMany(r => Enumerable.Range(boxCol * 3, 3).Select(c => (r, c))).ToList(),
                                "box");

                            if (!string.IsNullOrEmpty(solveStep.Explanation))
                            {
                                return solveStep;
                            }
                        }
                    }
                }
            }

            return solveStep;
        }
    }


}
