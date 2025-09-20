using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class XWing
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.XWing,
            HighlightedSquares = new HashSet<(int row, int col)>(),
            CandidatesRemovedSquares = new HashSet<(int row, int col)>()
        };

        // Row-based X-Wing
        for (int number = 1; number <= 9; number++)
        {
            var rowColumns = new List<(int row, List<int> cols)>();

            for (int rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                var cols = new List<int>();

                for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    if (squares[rowIndex, columnIndex].PossibleNumbers.Contains(number))
                    {
                        cols.Add(columnIndex);
                    }
                }

                if (cols.Count == 2)
                {
                    rowColumns.Add((rowIndex, cols));
                }
            }

            // Look for two rows with the same columns
            for (int i = 0; i < rowColumns.Count; i++)
            {
                for (int j = i + 1; j < rowColumns.Count; j++)
                {
                    if (rowColumns[i].cols.SequenceEqual(rowColumns[j].cols))
                    {
                        int rowIndex1 = rowColumns[i].row;
                        int rowIndex2 = rowColumns[j].row;
                        int columnIndex1 = rowColumns[i].cols[0];
                        int columnIndex2 = rowColumns[i].cols[1];

                        // Highlight the four corners
                        solveStep.HighlightedSquares.Add((rowIndex1, columnIndex1));
                        solveStep.HighlightedSquares.Add((rowIndex1, columnIndex2));
                        solveStep.HighlightedSquares.Add((rowIndex2, columnIndex1));
                        solveStep.HighlightedSquares.Add((rowIndex2, columnIndex2));

                        // Remove candidate from other rows in these columns
                        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                        {
                            if (rowIndex != rowIndex1 && rowIndex != rowIndex2)
                            {
                                if (squares[rowIndex, columnIndex1].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((rowIndex, columnIndex1));
                                }

                                if (squares[rowIndex, columnIndex2].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((rowIndex, columnIndex2));
                                }
                            }
                        }

                        if (solveStep.CandidatesRemovedSquares.Count > 0)
                        {
                            solveStep.Number = number;
                            solveStep.Explanation = $"X-Wing (rows): {number} appears in exactly two columns in two rows ({rowIndex1 + 1}, {rowIndex2 + 1}).{Environment.NewLine}Remove {number} from other rows in those columns.";
                            solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                            solveStep.CandidatesRemovedNumbers.Add(number);
                            return solveStep;
                        }

                        solveStep.HighlightedSquares.Clear();
                    }
                }
            }
        }

        // Column-based X-Wing
        for (int number = 1; number <= 9; number++)
        {
            var columnRows = new List<(int col, List<int> rows)>();

            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var rows = new List<int>();

                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    if (squares[rowIndex, columnIndex].PossibleNumbers.Contains(number))
                    {
                        rows.Add(rowIndex);
                    }
                }

                if (rows.Count == 2)
                {
                    columnRows.Add((columnIndex, rows));
                }
            }

            // Look for two columns with the same rows
            for (int i = 0; i < columnRows.Count; i++)
            {
                for (int j = i + 1; j < columnRows.Count; j++)
                {
                    if (columnRows[i].rows.SequenceEqual(columnRows[j].rows))
                    {
                        int columnIndex1 = columnRows[i].col;
                        int columnIndex2 = columnRows[j].col;
                        int rowIndex1 = columnRows[i].rows[0];
                        int rowIndex2 = columnRows[i].rows[1];

                        // Highlight the four corners
                        solveStep.HighlightedSquares.Add((rowIndex1, columnIndex1));
                        solveStep.HighlightedSquares.Add((rowIndex2, columnIndex1));
                        solveStep.HighlightedSquares.Add((rowIndex1, columnIndex2));
                        solveStep.HighlightedSquares.Add((rowIndex2, columnIndex2));

                        // Remove candidate from other columns in these rows
                        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                        {
                            if (columnIndex != columnIndex1 && columnIndex != columnIndex2)
                            {
                                if (squares[rowIndex1, columnIndex].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((rowIndex1, columnIndex));
                                }

                                if (squares[rowIndex2, columnIndex].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((rowIndex2, columnIndex));
                                }
                            }
                        }

                        if (solveStep.CandidatesRemovedSquares.Count > 0)
                        {
                            solveStep.Number = number;
                            solveStep.Explanation = $"X-Wing (columns): {number} appears in exactly two rows in two columns ({columnIndex1 + 1}, {columnIndex2 + 1}).{Environment.NewLine}Remove {number} from other columns in those rows.";
                            solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                            solveStep.CandidatesRemovedNumbers.Add(number);
                            return solveStep;
                        }

                        solveStep.HighlightedSquares.Clear();
                    }
                }
            }
        }

        return solveStep;
    }
}
