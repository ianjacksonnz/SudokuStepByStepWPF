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
        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int firstNumber = 1; firstNumber <= 8; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 9; secondNumber++)
                {
                    var pairColumns = new List<int>();

                    for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                    {
                        var square = squares[rowIndex, columnIndex];

                        if (square.PossibleNumbers.Contains(firstNumber) && square.PossibleNumbers.Contains(secondNumber))
                        {
                            pairColumns.Add(columnIndex);
                        }
                    }

                    if (pairColumns.Count == 2)
                    {
                        // Ensure firstNumber and secondNumber do NOT appear in any other square in the row
                        bool onlyInPair = true;

                        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                        {
                            if (!pairColumns.Contains(columnIndex))
                            {
                                var square = squares[rowIndex, columnIndex];

                                if (square.PossibleNumbers.Contains(firstNumber) || square.PossibleNumbers.Contains(secondNumber))
                                {
                                    onlyInPair = false;
                                    break;
                                }
                            }
                        }

                        if (onlyInPair && pairColumns.All(columnIndex => squares[rowIndex, columnIndex].PossibleNumbers.Count > 2))
                        {
                            solveStep.Number = 0;
                            solveStep.Explanation = $"Hidden Pair: Numbers {firstNumber} and {secondNumber} only appear together in two squares in row {rowIndex + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                            foreach (var column in pairColumns)
                            {
                                solveStep.HighlightedSquares.Add((rowIndex, column));

                                foreach (var num in squares[rowIndex, column].PossibleNumbers.ToList())
                                {
                                    if (num != firstNumber && num != secondNumber)
                                    {
                                        hiddenPairFound = true;
                                        solveStep.CandidatesRemovedSquares.Add((rowIndex, column));
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
        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
        {
            for (int firstNumber = 1; firstNumber <= 8; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 9; secondNumber++)
                {
                    var pairRows = new List<int>();

                    for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                    {
                        var square = squares[rowIndex, columnIndex];

                        if (square.PossibleNumbers.Contains(firstNumber) && square.PossibleNumbers.Contains(secondNumber))
                        {
                            pairRows.Add(rowIndex);
                        }
                    }

                    if (pairRows.Count == 2)
                    {
                        bool onlyInPair = true;

                        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                        {
                            if (!pairRows.Contains(rowIndex))
                            {
                                var square = squares[rowIndex, columnIndex];

                                if (square.PossibleNumbers.Contains(firstNumber) || square.PossibleNumbers.Contains(secondNumber))
                                {
                                    onlyInPair = false;
                                    break;
                                }
                            }
                        }

                        if (onlyInPair && pairRows.All(rowIndex => squares[rowIndex, columnIndex].PossibleNumbers.Count > 2))
                        {
                            solveStep.Number = 0;
                            solveStep.Explanation = $"Hidden Pair: Numbers {firstNumber} and {secondNumber} only appear together in two squares in column {columnIndex + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                            foreach (var rowIndex in pairRows)
                            {
                                solveStep.HighlightedSquares.Add((rowIndex, columnIndex));

                                foreach (var num in squares[rowIndex, columnIndex].PossibleNumbers.ToList())
                                {
                                    if (num != firstNumber && num != secondNumber)
                                    {
                                        hiddenPairFound = true;
                                        solveStep.CandidatesRemovedSquares.Add((rowIndex, columnIndex));
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
        for (int boxRowIndex = 0; boxRowIndex < 3; boxRowIndex++)
        {
            for (int boxColumnIndex = 0; boxColumnIndex < 3; boxColumnIndex++)
            {
                int startRowIndex = boxRowIndex * 3;
                int startColumnIndex = boxColumnIndex * 3;

                for (int firstNumber = 1; firstNumber <= 8; firstNumber++)
                {
                    for (int secondNumber = firstNumber + 1; secondNumber <= 9; secondNumber++)
                    {
                        var pairSquares = new List<(int row, int col)>();

                        for (int rowIndex = startRowIndex; rowIndex < startRowIndex + 3; rowIndex++)
                        {
                            for (int columnIndex = startColumnIndex; columnIndex < startColumnIndex + 3; columnIndex++)
                            {
                                var square = squares[rowIndex, columnIndex];

                                if (square.PossibleNumbers.Contains(firstNumber) && square.PossibleNumbers.Contains(secondNumber))
                                {
                                    pairSquares.Add((rowIndex, columnIndex));
                                }
                            }
                        }

                        if (pairSquares.Count == 2)
                        {
                            bool onlyInPair = true;

                            for (int rowIndex = startRowIndex; rowIndex < startRowIndex + 3; rowIndex++)
                            {
                                for (int columnIndex = startColumnIndex; columnIndex < startColumnIndex + 3; columnIndex++)
                                {
                                    if (!pairSquares.Contains((rowIndex, columnIndex)))
                                    {
                                        var square = squares[rowIndex, columnIndex];

                                        if (square.PossibleNumbers.Contains(firstNumber) || square.PossibleNumbers.Contains(secondNumber))
                                        {
                                            onlyInPair = false;
                                            break;
                                        }
                                    }
                                }

                                if (!onlyInPair) break;
                            }

                            if (onlyInPair && pairSquares.All(cell => squares[cell.row, cell.col].PossibleNumbers.Count > 2))
                            {
                                solveStep.Number = 0;
                                solveStep.Explanation = $"Hidden Pair: Numbers {firstNumber} and {secondNumber} only appear together in two squares in the box.{Environment.NewLine}Remove other candidates from those squares.";

                                foreach (var cell in pairSquares)
                                {
                                    solveStep.HighlightedSquares.Add(cell);

                                    foreach (var number in squares[cell.row, cell.col].PossibleNumbers.ToList())
                                    {
                                        if (number != firstNumber && number != secondNumber)
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
