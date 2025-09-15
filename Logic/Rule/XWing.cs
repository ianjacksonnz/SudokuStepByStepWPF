using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;
using System.Collections.Generic;
using System.Linq;

namespace SudokuStepByStep.Logic.Rule;

public static class XWing
{
    public static SolveStep Run(SudokuSquareModel[,] squares)
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

            for (int row = 0; row < 9; row++)
            {
                var cols = new List<int>();

                for (int col = 0; col < 9; col++)
                {
                    if (squares[row, col].PossibleNumbers.Contains(number))
                    {
                        cols.Add(col);
                    }
                }

                if (cols.Count == 2)
                {
                    rowColumns.Add((row, cols));
                }
            }

            // Look for two rows with the same columns
            for (int i = 0; i < rowColumns.Count; i++)
            {
                for (int j = i + 1; j < rowColumns.Count; j++)
                {
                    if (rowColumns[i].cols.SequenceEqual(rowColumns[j].cols))
                    {
                        int row1 = rowColumns[i].row;
                        int row2 = rowColumns[j].row;
                        int col1 = rowColumns[i].cols[0];
                        int col2 = rowColumns[i].cols[1];

                        // Highlight the four corners
                        solveStep.HighlightedSquares.Add((row1, col1));
                        solveStep.HighlightedSquares.Add((row1, col2));
                        solveStep.HighlightedSquares.Add((row2, col1));
                        solveStep.HighlightedSquares.Add((row2, col2));

                        // Remove candidate from other rows in these columns
                        for (int row = 0; row < 9; row++)
                        {
                            if (row != row1 && row != row2)
                            {
                                if (squares[row, col1].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((row, col1));
                                }

                                if (squares[row, col2].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((row, col2));
                                }
                            }
                        }

                        if (solveStep.CandidatesRemovedSquares.Count > 0)
                        {
                            solveStep.Number = number;
                            solveStep.Explanation = $"X-Wing (rows): {number} appears in exactly two columns in two rows ({row1 + 1}, {row2 + 1}).{Environment.NewLine}Remove {number} from other rows in those columns.";
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

            for (int col = 0; col < 9; col++)
            {
                var rows = new List<int>();

                for (int row = 0; row < 9; row++)
                {
                    if (squares[row, col].PossibleNumbers.Contains(number))
                    {
                        rows.Add(row);
                    }
                }

                if (rows.Count == 2)
                {
                    columnRows.Add((col, rows));
                }
            }

            // Look for two columns with the same rows
            for (int i = 0; i < columnRows.Count; i++)
            {
                for (int j = i + 1; j < columnRows.Count; j++)
                {
                    if (columnRows[i].rows.SequenceEqual(columnRows[j].rows))
                    {
                        int col1 = columnRows[i].col;
                        int col2 = columnRows[j].col;
                        int row1 = columnRows[i].rows[0];
                        int row2 = columnRows[i].rows[1];

                        // Highlight the four corners
                        solveStep.HighlightedSquares.Add((row1, col1));
                        solveStep.HighlightedSquares.Add((row2, col1));
                        solveStep.HighlightedSquares.Add((row1, col2));
                        solveStep.HighlightedSquares.Add((row2, col2));

                        // Remove candidate from other columns in these rows
                        for (int col = 0; col < 9; col++)
                        {
                            if (col != col1 && col != col2)
                            {
                                if (squares[row1, col].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((row1, col));
                                }

                                if (squares[row2, col].PossibleNumbers.Contains(number))
                                {
                                    solveStep.CandidatesRemovedSquares.Add((row2, col));
                                }
                            }
                        }

                        if (solveStep.CandidatesRemovedSquares.Count > 0)
                        {
                            solveStep.Number = number;
                            solveStep.Explanation = $"X-Wing (columns): {number} appears in exactly two rows in two columns ({col1 + 1}, {col2 + 1}).{Environment.NewLine}Remove {number} from other columns in those rows.";
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
