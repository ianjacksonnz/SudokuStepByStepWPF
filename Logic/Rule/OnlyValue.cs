using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class OnlyValue
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.OnlyValue,
        };

        var gridPossibleNumbers = GridHelper.GetPossibleNumbers(squares);

        // --- rows ---
        for (int row = 0; row < 9; row++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleColumns = new List<int>();

                for (int column = 0; column < 9; column++)
                {
                    var square = squares[row, column];

                    if (square.PossibleNumbers.Count > 0 &&
                        square.PossibleNumbers.Contains(number))
                    {
                        possibleColumns.Add(column);
                    }
                }

                if (possibleColumns.Count == 1)
                {
                    var solvedColumn = possibleColumns[0];

                    solveStep.Solved = true;
                    solveStep.Number = number;
                    solveStep.Row = row;
                    solveStep.Column = solvedColumn;
                    solveStep.Explanation = $"The number {number} can only fit in this square in row {row + 1}.";

                    for (int column = 0; column < 9; column++)
                    {
                        if (column != solvedColumn)
                        {
                            solveStep.HighlightedSquares.Add((row, column));
                        }
                    }

                    return solveStep;
                }
            }
        }

        // --- columns ---
        for (int column = 0; column < 9; column++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleRows = new List<int>();

                for (int row = 0; row < 9; row++)
                {
                    var square = squares[row, column];
                    var isSafe = RulesHelper.IsPossibleNumber(gridPossibleNumbers, row, column, number);

                    if (square.PossibleNumbers.Count > 0 &&
                        square.PossibleNumbers.Contains(number))
                    {
                        possibleRows.Add(row);
                    }
                }

                if (possibleRows.Count == 1)
                {
                    var solvedRow = possibleRows[0];

                    solveStep.Solved = true;
                    solveStep.Number = number;
                    solveStep.Row = solvedRow;
                    solveStep.Column = column;
                    solveStep.Explanation = $"The number {number} can only fit in this square in column {column + 1}.";

                    for (int row = 0; row < 9; row++)
                    {
                        if (row != solvedRow)
                        {
                            solveStep.HighlightedSquares.Add((row, column));
                        }
                    }

                    return solveStep;
                }
            }
        }

        // --- grids ---
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    var possibleSquares = new List<(int row, int column)>();

                    for (int row = boxRow * 3; row < boxRow * 3 + 3; row++)
                    {
                        for (int column = boxCol * 3; column < boxCol * 3 + 3; column++)
                        {
                            var square = squares[row, column];
                            var isSafe = RulesHelper.IsPossibleNumber(gridPossibleNumbers, row, column, number);

                            if (square.PossibleNumbers.Count > 0 &&
                                square.PossibleNumbers.Contains(number) && isSafe)
                            {
                                possibleSquares.Add((row, column));
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

                        for (int r = 0; r < 9; r++)
                        {
                            for (int c = 0; c < 9; c++)
                            {
                                if ((r != solvedRow) && (c != solvedColumn))
                                {
                                    solveStep.HighlightedSquares.Add((r, c));
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
