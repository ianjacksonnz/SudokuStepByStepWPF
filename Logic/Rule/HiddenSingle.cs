using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

/// <summary>
/// The number can go in only on place in the row, column or grid
/// </summary>
public static class HiddenSingle
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.HiddenSingle,
        };

        var gridPossibleNumbers = GridHelper.GetPossibleNumbers(squares);

        // --- rows ---
        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleColumns = new List<int>();

                for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    var square = squares[rowIndex, columnIndex];

                    if (square.PossibleNumbers.Count > 0 &&
                        square.PossibleNumbers.Contains(number))
                    {
                        possibleColumns.Add(columnIndex);
                    }
                }

                if (possibleColumns.Count == 1)
                {
                    var solvedColumn = possibleColumns[0];

                    solveStep.Solved = true;
                    solveStep.Number = number;
                    solveStep.Row = rowIndex;
                    solveStep.Column = solvedColumn;
                    solveStep.Explanation = $"The number {number} can only fit in this square in row {rowIndex + 1}.";

                    for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                    {
                        if (columnIndex != solvedColumn)
                        {
                            solveStep.HighlightedSquares.Add((rowIndex, columnIndex));
                        }
                    }

                    return solveStep;
                }
            }
        }

        // --- columns ---
        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleRows = new List<int>();

                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    var square = squares[rowIndex, columnIndex];

                    if (square.PossibleNumbers.Count > 0 &&
                        square.PossibleNumbers.Contains(number))
                    {
                        possibleRows.Add(rowIndex);
                    }
                }

                if (possibleRows.Count == 1)
                {
                    var solvedRow = possibleRows[0];

                    solveStep.Solved = true;
                    solveStep.Number = number;
                    solveStep.Row = solvedRow;
                    solveStep.Column = columnIndex;
                    solveStep.Explanation = $"The number {number} can only fit in this square in column {columnIndex + 1}.";

                    for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                    {
                        if (rowIndex != solvedRow)
                        {
                            solveStep.HighlightedSquares.Add((rowIndex, columnIndex));
                        }
                    }

                    return solveStep;
                }
            }
        }

        // --- 3*3 boxes ---
        for (int boxRowIndex = 0; boxRowIndex < 3; boxRowIndex++)
        {
            for (int boxColumnIndex = 0; boxColumnIndex < 3; boxColumnIndex++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    var possibleSquares = new List<(int row, int column)>();

                    for (int rowIndex = boxRowIndex * 3; rowIndex < boxRowIndex * 3 + 3; rowIndex++)
                    {
                        for (int columnIndex = boxColumnIndex * 3; columnIndex < boxColumnIndex * 3 + 3; columnIndex++)
                        {
                            var square = squares[rowIndex, columnIndex];

                            if (square.PossibleNumbers.Count > 0 &&
                                square.PossibleNumbers.Contains(number))
                            {
                                possibleSquares.Add((rowIndex, columnIndex));
                            }
                        }
                    }

                    if (possibleSquares.Count == 1)
                    {
                        var solvedRow = possibleSquares[0].row;
                        var solvedColumn = possibleSquares[0].column;

                        solveStep.Number = number;
                        solveStep.Row = solvedRow;
                        solveStep.Column = solvedColumn;
                        solveStep.Explanation = $"The number {number} can only fit in this square in its 3x3 grid.";

                        int startRowIndex = (solvedRow / 3) * 3;
                        int startColumnIndex = (solvedColumn / 3) * 3;

                        for (int rowIndex = startRowIndex; rowIndex < startRowIndex + 3; rowIndex++)
                        {
                            for (int columnIndex = startColumnIndex; columnIndex < startColumnIndex + 3; columnIndex++)
                            {
                                if (rowIndex != solvedRow || columnIndex != solvedColumn)
                                {
                                    solveStep.HighlightedSquares.Add((rowIndex, columnIndex));
                                }
                            }
                        }

                        solveStep.Solved = true;
                        return solveStep;
                    }
                }
            }
        }

        return solveStep;
    }
}
