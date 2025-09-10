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
            for (int row = 0; row < 9; row++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(column => squares[row, column].PossibleNumbers.Count >= 2 && squares[row, column].PossibleNumbers.Count <= groupSize)
                    .Select(col => (row, col)).ToList();

                if (cells.Count >= groupSize)
                {
                    foreach (var group in cells.Combinations(groupSize))
                    {
                        CheckGroup(group, Enumerable.Range(0, 9).Select(column => (row, column)).ToList(),
                                   Enums.SquareGroupType.Row, squares, solveStep, groupSize, row);

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                            return solveStep;
                    }
                }
            }

            // Check columns
            for (int column = 0; column < 9; column++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(row => squares[row, column].PossibleNumbers.Count >= 2 && squares[row, column].PossibleNumbers.Count <= groupSize)
                    .Select(row => (row, column)).ToList();

                if (cells.Count >= groupSize)
                {
                    foreach (var group in cells.Combinations(groupSize))
                    {
                        CheckGroup(group, Enumerable.Range(0, 9).Select(row => (row, column)).ToList(),
                                   Enums.SquareGroupType.Column, squares, solveStep, groupSize, 0, column);

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                            return solveStep;
                    }
                }
            }

            // Check boxes
            for (int boxRow = 0; boxRow < 3; boxRow++)
            {
                for (int boxColumn = 0; boxColumn < 3; boxColumn++)
                {
                    var boxCells = new List<(int row, int col)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxColumn * 3; c < boxColumn * 3 + 3; c++)
                        {
                            if (squares[r, c].PossibleNumbers.Count >= 2 && squares[r, c].PossibleNumbers.Count <= groupSize)
                            {
                                boxCells.Add((r, c));
                            }
                        }
                    }

                    if (boxCells.Count >= groupSize)
                    {
                        foreach (var group in boxCells.Combinations(groupSize))
                        {
                            CheckGroup(group,
                                Enumerable.Range(boxRow * 3, 3)
                                    .SelectMany(r => Enumerable.Range(boxColumn * 3, 3).Select(c => (r, c))).ToList(),
                                Enums.SquareGroupType.Box, squares, solveStep, groupSize, boxRow, boxColumn);

                            if (!string.IsNullOrEmpty(solveStep.Explanation))
                                return solveStep;
                        }
                    }
                }
            }

            return solveStep;
        }

        private static void PopulateHighlightedSquares(Enums.SquareGroupType groupType,
                                                       HashSet<(int row, int col)> candidatesRemovedSquares,
                                                       SolveStep solveStep, int groupIndexRow = 0, int groupIndexCol = 0)
        {
            switch (groupType)
            {
                case Enums.SquareGroupType.Row:
                    for (int column = 0; column < 9; column++)
                        if (!candidatesRemovedSquares.Contains((groupIndexRow, column)))
                            solveStep.HighlightedSquares.Add((groupIndexRow, column));
                    break;

                case Enums.SquareGroupType.Column:
                    for (int row = 0; row < 9; row++)
                        if (!candidatesRemovedSquares.Contains((row, groupIndexCol)))
                            solveStep.HighlightedSquares.Add((row, groupIndexCol));
                    break;

                case Enums.SquareGroupType.Box:
                    int startRow = groupIndexRow * 3;
                    int startColumn = groupIndexCol * 3;

                    for (int r = startRow; r < startRow + 3; r++)
                        for (int c = startColumn; c < startColumn + 3; c++)
                            if (!candidatesRemovedSquares.Contains((r, c)))
                                solveStep.HighlightedSquares.Add((r, c));
                    break;
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
                groupCandidates.UnionWith(squares[cell.row, cell.col].PossibleNumbers);

            if (groupCandidates.Count == groupSize &&
                cells.All(cell => squares[cell.row, cell.col].PossibleNumbers.All(n => groupCandidates.Contains(n))))
            {
                foreach (var cell in allCells.Except(cells))
                {
                    foreach (var n in groupCandidates)
                    {
                        if (squares[cell.row, cell.col].PossibleNumbers.Contains(n))
                        {
                            solveStep.CandidatesRemovedSquares.Add(cell);

                            if (!solveStep.CandidatesRemovedNumbers.Contains(n))
                                solveStep.CandidatesRemovedNumbers.Add(n);
                        }
                    }
                }

                if (solveStep.CandidatesRemovedSquares.Count > 0)
                {
                    PopulateHighlightedSquares(groupType, solveStep.CandidatesRemovedSquares, solveStep, groupIndexRow, groupIndexCol);
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
