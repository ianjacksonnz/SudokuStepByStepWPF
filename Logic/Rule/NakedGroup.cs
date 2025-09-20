using SudokuStepByStep.Common;
using SudokuStepByStep.Models;
using System.Collections.Generic;
using System.Linq;

namespace SudokuStepByStep.Logic.Rule
{
    public static class NakedGroup
    {
        public static SolveStep Run(SudokuSquare[,] squares, int groupSize, Enums.SolvingRule rule)
        {
            var solveStep = new SolveStep()
            {
                Rule = rule,
                HighlightedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedNumbers = new List<int>()
            };

            // Check rows
            for (int rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(column => squares[rowIndex, column].PossibleNumbers.Count >= 2 && squares[rowIndex, column].PossibleNumbers.Count <= groupSize)
                    .Select(col => (rowIndex, col)).ToList();

                if (cells.Count >= groupSize)
                {
                    foreach (var group in cells.Combinations(groupSize))
                    {
                        CheckGroup(group, Enumerable.Range(0, 9).Select(column => (rowIndex, column)).ToList(),
                                   Enums.SquareGroupType.Row, squares, solveStep, groupSize, rowIndex);

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                            return solveStep;
                    }
                }
            }

            // Check columns
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(row => squares[row, columnIndex].PossibleNumbers.Count >= 2 && squares[row, columnIndex].PossibleNumbers.Count <= groupSize)
                    .Select(row => (row, columnIndex)).ToList();

                if (cells.Count >= groupSize)
                {
                    foreach (var group in cells.Combinations(groupSize))
                    {
                        CheckGroup(group, Enumerable.Range(0, 9).Select(row => (row, columnIndex)).ToList(),
                                   Enums.SquareGroupType.Column, squares, solveStep, groupSize, 0, columnIndex);

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                        {
                            return solveStep;
                        }
                    }
                }
            }

            // Check boxes
            for (int boxRowIndex = 0; boxRowIndex < 3; boxRowIndex++)
            {
                for (int boxColumnIndex = 0; boxColumnIndex < 3; boxColumnIndex++)
                {
                    var boxCells = new List<(int row, int col)>();

                    for (int rowIndex = boxRowIndex * 3; rowIndex < boxRowIndex * 3 + 3; rowIndex++)
                    {
                        for (int columnIndex = boxColumnIndex * 3; columnIndex < boxColumnIndex * 3 + 3; columnIndex++)
                        {
                            if (squares[rowIndex, columnIndex].PossibleNumbers.Count >= 2 && squares[rowIndex, columnIndex].PossibleNumbers.Count <= groupSize)
                            {
                                boxCells.Add((rowIndex, columnIndex));
                            }
                        }
                    }

                    if (boxCells.Count >= groupSize)
                    {
                        foreach (var group in boxCells.Combinations(groupSize))
                        {
                            CheckGroup(group,
                                Enumerable.Range(boxRowIndex * 3, 3)
                                    .SelectMany(r => Enumerable.Range(boxColumnIndex * 3, 3).Select(c => (r, c))).ToList(),
                                Enums.SquareGroupType.Box, squares, solveStep, groupSize, boxRowIndex, boxColumnIndex);

                            if (!string.IsNullOrEmpty(solveStep.Explanation))
                                return solveStep;
                        }
                    }
                }
            }

            return solveStep;
        }

        private static void PopulateHighlightedSquares(List<(int row, int col)> cells, SolveStep solveStep)
        {
            foreach (var cell in cells)
            {
                solveStep.HighlightedSquares.Add(cell);
            }
        }

        private static void CheckGroup(List<(int row, int col)> cells,
                                       List<(int row, int col)> allCells,
                                       Enums.SquareGroupType groupType,
                                       SudokuSquare[,] squares,
                                       SolveStep solveStep,
                                       int groupSize,
                                       int groupIndexRow = 0, int groupIndexCol = 0)
        {
            var groupCandidates = new HashSet<int>();

            foreach (var cell in cells)
            {
                groupCandidates.UnionWith(squares[cell.row, cell.col].PossibleNumbers);
            }

            if (groupCandidates.Count == groupSize &&
                cells.All(cell => squares[cell.row, cell.col].PossibleNumbers.All(n => groupCandidates.Contains(n))))
            {
                foreach (var cell in allCells.Except(cells))
                {
                    foreach (var number in groupCandidates)
                    {
                        if (squares[cell.row, cell.col].PossibleNumbers.Contains(number))
                        {
                            solveStep.CandidatesRemovedSquares.Add(cell);

                            if (!solveStep.CandidatesRemovedNumbers.Contains(number))
                                solveStep.CandidatesRemovedNumbers.Add(number);
                        }
                    }
                }

                if (solveStep.CandidatesRemovedSquares.Count > 0)
                {
                    PopulateHighlightedSquares(cells, solveStep);

                    solveStep.CandidatesRemovedInNonHighlightedSquares = true;

                    string groupName = groupSize == 2 ? "pair" : groupSize == 3 ? "triple" : "quad";
                    var typeName = groupType.ToString().ToLower();
                    var formattedCandidates = FormatCandidates(groupCandidates);

                    solveStep.Explanation =
                        $"Naked {groupName}: {formattedCandidates} " +
                        $"only appear in {groupSize} squares in the same {typeName}.{Environment.NewLine}" +
                        $"Remove these numbers from other squares in the {typeName}.";
                }
            }
        }

        private static string FormatCandidates(HashSet<int> candidates)
        {
            return candidates.Count switch
            {
                2 => $"Numbers {candidates.ElementAt(0)} and {candidates.ElementAt(1)}",
                3 => $"Numbers {candidates.ElementAt(0)}, {candidates.ElementAt(1)} and {candidates.ElementAt(2)}",
                4 => $"Numbers {candidates.ElementAt(0)}, {candidates.ElementAt(1)}, {candidates.ElementAt(2)} and {candidates.ElementAt(3)}",
                _ => throw new InvalidOperationException("Unexpected number of candidates")
            };
        }
    }
}
