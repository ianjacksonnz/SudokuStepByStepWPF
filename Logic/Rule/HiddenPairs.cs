using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

/// <summary>
/// Apply the hidden pairs rule
/// If two numbers appear only in the same two cells of a row, column, or box,
/// then those two cells can only contain those two numbers.
/// </summary>
public static class HiddenPairs
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        bool hiddenPairFound = false;

        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.HiddenPairs,
            HighlightedSquares = new HashSet<(int row, int col)>(),
            CandidatesRemovedSquares = new HashSet<(int row, int col)>()
        };

        // Check each row
        for (int row = 0; row < 9; row++)
        {
            for (int firstNumber = 1; firstNumber <= 8; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 9; secondNumber++)
                {
                    var pairColumns = new List<int>();

                    for (int column = 0; column < 9; column++)
                    {
                        var square = squares[row, column];

                        if (square.PossibleNumbers.Contains(firstNumber) && square.PossibleNumbers.Contains(secondNumber))
                        {
                            pairColumns.Add(column);
                        }
                    }

                    if (pairColumns.Count == 2)
                    {
                        // Ensure firstNumber and secondNumber do NOT appear in any other square in the row
                        bool onlyInPair = true;

                        for (int column = 0; column < 9; column++)
                        {
                            if (!pairColumns.Contains(column))
                            {
                                var square = squares[row, column];

                                if (square.PossibleNumbers.Contains(firstNumber) || square.PossibleNumbers.Contains(secondNumber))
                                {
                                    onlyInPair = false;
                                    break;
                                }
                            }
                        }

                        if (onlyInPair && pairColumns.All(col => squares[row, col].PossibleNumbers.Count > 2))
                        {
                            solveStep.Number = 0;
                            solveStep.Explanation = $"Hidden Pair: Numbers {firstNumber} and {secondNumber} only appear together in two squares in row {row + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                            foreach (var column in pairColumns)
                            {
                                solveStep.HighlightedSquares.Add((row, column));

                                foreach (var num in squares[row, column].PossibleNumbers.ToList())
                                {
                                    if (num != firstNumber && num != secondNumber)
                                    {
                                        hiddenPairFound = true;
                                        solveStep.CandidatesRemovedSquares.Add((row, column));
                                        solveStep.CandidatesRemovedInHighlightedSquares = true;
                                    }
                                }
                            }

                            if (hiddenPairFound)
                            {
                                solveStep.CandidatesRemovedNumbers = RulesHelper.RemoveOtherNumbers(firstNumber, secondNumber);
                                return solveStep;
                            }

                        }
                    }
                }
            }
        }

        // Check each column
        for (int column = 0; column < 9; column++)
        {
            for (int firstNumber = 1; firstNumber <= 8; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 9; secondNumber++)
                {
                    var pairRows = new List<int>();

                    for (int row = 0; row < 9; row++)
                    {
                        var square = squares[row, column];

                        if (square.PossibleNumbers.Contains(firstNumber) && square.PossibleNumbers.Contains(secondNumber))
                        {
                            pairRows.Add(row);
                        }
                    }

                    if (pairRows.Count == 2)
                    {
                        bool onlyInPair = true;

                        for (int row = 0; row < 9; row++)
                        {
                            if (!pairRows.Contains(row))
                            {
                                var square = squares[row, column];

                                if (square.PossibleNumbers.Contains(firstNumber) || square.PossibleNumbers.Contains(secondNumber))
                                {
                                    onlyInPair = false;
                                    break;
                                }
                            }
                        }

                        if (onlyInPair && pairRows.All(row => squares[row, column].PossibleNumbers.Count > 2))
                        {
                            solveStep.Number = 0;
                            solveStep.Explanation = $"Hidden Pair: Numbers {firstNumber} and {secondNumber} only appear together in two squares in column {column + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                            foreach (var row in pairRows)
                            {
                                solveStep.HighlightedSquares.Add((row, column));

                                foreach (var num in squares[row, column].PossibleNumbers.ToList())
                                {
                                    if (num != firstNumber && num != secondNumber)
                                    {
                                        hiddenPairFound = true;
                                        solveStep.CandidatesRemovedSquares.Add((row, column));
                                        solveStep.CandidatesRemovedInHighlightedSquares = true;
                                    }
                                }
                            }

                            if (hiddenPairFound)
                            {
                                solveStep.CandidatesRemovedNumbers = RulesHelper.RemoveOtherNumbers(firstNumber, secondNumber);
                                return solveStep;
                            }
                        }
                    }
                }
            }
        }

        // Check each 3x3 box
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxColumn = 0; boxColumn < 3; boxColumn++)
            {
                int startRow = boxRow * 3;
                int startColumn = boxColumn * 3;

                for (int firstNumber = 1; firstNumber <= 8; firstNumber++)
                {
                    for (int secondNumber = firstNumber + 1; secondNumber <= 9; secondNumber++)
                    {
                        var pairCells = new List<(int row, int col)>();

                        for (int r = startRow; r < startRow + 3; r++)
                        {
                            for (int c = startColumn; c < startColumn + 3; c++)
                            {
                                var square = squares[r, c];

                                if (square.PossibleNumbers.Contains(firstNumber) && square.PossibleNumbers.Contains(secondNumber))
                                {
                                    pairCells.Add((r, c));
                                }
                            }
                        }

                        if (pairCells.Count == 2)
                        {
                            bool onlyInPair = true;

                            for (int r = startRow; r < startRow + 3; r++)
                            {
                                for (int c = startColumn; c < startColumn + 3; c++)
                                {
                                    if (!pairCells.Contains((r, c)))
                                    {
                                        var square = squares[r, c];

                                        if (square.PossibleNumbers.Contains(firstNumber) || square.PossibleNumbers.Contains(secondNumber))
                                        {
                                            onlyInPair = false;
                                            break;
                                        }
                                    }
                                }

                                if (!onlyInPair) break;
                            }

                            if (onlyInPair && pairCells.All(cell => squares[cell.row, cell.col].PossibleNumbers.Count > 2))
                            {
                                solveStep.Number = 0;
                                solveStep.Explanation = $"Hidden Pair: Numbers {firstNumber} and {secondNumber} only appear together in two squares in the box.{Environment.NewLine}Remove other candidates from those squares.";

                                foreach (var cell in pairCells)
                                {
                                    solveStep.HighlightedSquares.Add(cell);

                                    foreach (var num in squares[cell.row, cell.col].PossibleNumbers.ToList())
                                    {
                                        if (num != firstNumber && num != secondNumber)
                                        {
                                            hiddenPairFound = true;
                                            solveStep.CandidatesRemovedSquares.Add(cell);
                                            solveStep.CandidatesRemovedInHighlightedSquares = true;
                                        }
                                    }
                                }

                                if (hiddenPairFound)
                                {
                                    solveStep.CandidatesRemovedNumbers = RulesHelper.RemoveOtherNumbers(firstNumber, secondNumber);
                                    return solveStep;
                                }

                            }
                        }
                    }
                }
            }
        }

        // No hidden pair found
        return solveStep;
    }
}

