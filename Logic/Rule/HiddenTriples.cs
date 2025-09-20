using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

/// <summary>
/// Apply the hidden triples rule
/// If three numbers appear only in the same three cells of a row, column, or box,
/// then those three cells can only contain those three numbers.
/// </summary>
public static class HiddenTriples
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        bool hiddenTripleFound = false;

        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.HiddenTriples,
            HighlightedSquares = new HashSet<(int row, int col)>(),
            CandidatesRemovedSquares = new HashSet<(int row, int col)>()
        };

        // --- Check rows ---
        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int firstNumber = 1; firstNumber <= 7; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 8; secondNumber++)
                {
                    for (int thirdNumber = secondNumber + 1; thirdNumber <= 9; thirdNumber++)
                    {
                        var tripleColumns = new List<int>();

                        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                        {
                            var square = squares[rowIndex, columnIndex];

                            if (square.PossibleNumbers.Contains(firstNumber) &&
                                square.PossibleNumbers.Contains(secondNumber) &&
                                square.PossibleNumbers.Contains(thirdNumber))
                            {
                                tripleColumns.Add(columnIndex);
                            }
                        }

                        if (tripleColumns.Count == 3)
                        {
                            // Ensure these numbers do NOT appear in any other square in the row
                            bool onlyInTriple = true;

                            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                            {
                                if (!tripleColumns.Contains(columnIndex))
                                {
                                    var square = squares[rowIndex, columnIndex];
                                    if (square.PossibleNumbers.Contains(firstNumber) ||
                                        square.PossibleNumbers.Contains(secondNumber) ||
                                        square.PossibleNumbers.Contains(thirdNumber))
                                    {
                                        onlyInTriple = false;
                                        break;
                                    }
                                }
                            }

                            if (onlyInTriple && tripleColumns.All(col => squares[rowIndex, col].PossibleNumbers.Count > 3))
                            {
                                solveStep.Explanation =
                                    $"Hidden Triple: Numbers {firstNumber}, {secondNumber} and {thirdNumber} only appear together in three squares in row {rowIndex + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                                foreach (var column in tripleColumns)
                                {
                                    solveStep.HighlightedSquares.Add((rowIndex, column));

                                    foreach (var number in squares[rowIndex, column].PossibleNumbers.ToList())
                                    {
                                        if (number != firstNumber && number != secondNumber && number != thirdNumber)
                                        {
                                            hiddenTripleFound = true;
                                            solveStep.CandidatesRemovedSquares.Add((rowIndex, column));
                                            solveStep.CandidatesRemovedInHighlightedSquares = true;
                                        }
                                    }
                                }

                                if (hiddenTripleFound)
                                {
                                    solveStep.CandidatesRemovedNumbers = RulesHelper.RemoveOtherNumbers(firstNumber, secondNumber, thirdNumber);
                                    return solveStep;
                                }
                            }
                        }
                    }
                }
            }
        }

        // --- Check columns ---
        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
        {
            for (int firstNumber = 1; firstNumber <= 7; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 8; secondNumber++)
                {
                    for (int thirdNumber = secondNumber + 1; thirdNumber <= 9; thirdNumber++)
                    {
                        var tripleRows = new List<int>();

                        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                        {
                            var square = squares[rowIndex, columnIndex];

                            if (square.PossibleNumbers.Contains(firstNumber) &&
                                square.PossibleNumbers.Contains(secondNumber) &&
                                square.PossibleNumbers.Contains(thirdNumber))
                            {
                                tripleRows.Add(rowIndex);
                            }
                        }

                        if (tripleRows.Count == 3)
                        {
                            bool onlyInTriple = true;

                            for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                            {
                                if (!tripleRows.Contains(rowIndex))
                                {
                                    var square = squares[rowIndex, columnIndex];

                                    if (square.PossibleNumbers.Contains(firstNumber) ||
                                        square.PossibleNumbers.Contains(secondNumber) ||
                                        square.PossibleNumbers.Contains(thirdNumber))
                                    {
                                        onlyInTriple = false;
                                        break;
                                    }
                                }
                            }

                            if (onlyInTriple && tripleRows.All(r => squares[r, columnIndex].PossibleNumbers.Count > 3))
                            {
                                solveStep.Explanation =
                                    $"Hidden Triple: Numbers {firstNumber}, {secondNumber} and {thirdNumber} only appear together in three squares in column {columnIndex + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                                foreach (var rowIndex in tripleRows)
                                {
                                    solveStep.HighlightedSquares.Add((rowIndex, columnIndex));

                                    foreach (var number in squares[rowIndex, columnIndex].PossibleNumbers.ToList())
                                    {
                                        if (number != firstNumber && number != secondNumber && number != thirdNumber)
                                        {
                                            hiddenTripleFound = true;
                                            solveStep.CandidatesRemovedSquares.Add((rowIndex, columnIndex));
                                            solveStep.CandidatesRemovedInHighlightedSquares = true;
                                        }
                                    }
                                }

                                if (hiddenTripleFound)
                                {
                                    solveStep.CandidatesRemovedNumbers = RulesHelper.RemoveOtherNumbers(firstNumber, secondNumber, thirdNumber);
                                    return solveStep;
                                }
                            }
                        }
                    }
                }
            }
        }

        // --- Check 3x3 boxes ---
        for (int boxRowIndex = 0; boxRowIndex < 3; boxRowIndex++)
        {
            for (int boxColumnIndex = 0; boxColumnIndex < 3; boxColumnIndex++)
            {
                int startRowIndex = boxRowIndex * 3;
                int startColumnIndex = boxColumnIndex * 3;

                for (int firstNumber = 1; firstNumber <= 7; firstNumber++)
                {
                    for (int secondNumber = firstNumber + 1; secondNumber <= 8; secondNumber++)
                    {
                        for (int thirdNumber = secondNumber + 1; thirdNumber <= 9; thirdNumber++)
                        {
                            var tripleCells = new List<(int row, int col)>();

                            for (int rowIndex = startRowIndex; rowIndex < startRowIndex + 3; rowIndex++)
                            {
                                for (int columnIndex = startColumnIndex; columnIndex < startColumnIndex + 3; columnIndex++)
                                {
                                    var square = squares[rowIndex, columnIndex];

                                    if (square.PossibleNumbers.Contains(firstNumber) &&
                                        square.PossibleNumbers.Contains(secondNumber) &&
                                        square.PossibleNumbers.Contains(thirdNumber))
                                    {
                                        tripleCells.Add((rowIndex, columnIndex));
                                    }
                                }
                            }

                            if (tripleCells.Count == 3)
                            {
                                bool onlyInTriple = true;

                                for (int rowIndex = startRowIndex; rowIndex < startRowIndex + 3; rowIndex++)
                                {
                                    for (int columnIndex = startColumnIndex; columnIndex < startColumnIndex + 3; columnIndex++)
                                    {
                                        if (!tripleCells.Contains((rowIndex, columnIndex)))
                                        {
                                            var square = squares[rowIndex, columnIndex];

                                            if (square.PossibleNumbers.Contains(firstNumber) ||
                                                square.PossibleNumbers.Contains(secondNumber) ||
                                                square.PossibleNumbers.Contains(thirdNumber))
                                            {
                                                onlyInTriple = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (!onlyInTriple) break;
                                }

                                if (onlyInTriple && tripleCells.All(cell => squares[cell.row, cell.col].PossibleNumbers.Count > 3))
                                {
                                    solveStep.Explanation =
                                        $"Hidden Triple: Numbers {firstNumber}, {secondNumber} and {thirdNumber} only appear together in three squares in the box.{Environment.NewLine}Remove other candidates from those squares.";

                                    foreach (var cell in tripleCells)
                                    {
                                        solveStep.HighlightedSquares.Add(cell);

                                        foreach (var number in squares[cell.row, cell.col].PossibleNumbers.ToList())
                                        {
                                            if (number != firstNumber && number != secondNumber && number != thirdNumber)
                                            {
                                                hiddenTripleFound = true;
                                                solveStep.CandidatesRemovedSquares.Add(cell);
                                                solveStep.CandidatesRemovedInHighlightedSquares = true;
                                            }
                                        }
                                    }

                                    if (hiddenTripleFound)
                                    {
                                        solveStep.CandidatesRemovedNumbers = RulesHelper.RemoveOtherNumbers(firstNumber, secondNumber, thirdNumber);
                                        return solveStep;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // No hidden triple found
        return solveStep;
    }
}
