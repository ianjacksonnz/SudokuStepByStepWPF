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
        for (int row = 0; row < 9; row++)
        {
            for (int firstNumber = 1; firstNumber <= 7; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 8; secondNumber++)
                {
                    for (int thirdNumber = secondNumber + 1; thirdNumber <= 9; thirdNumber++)
                    {
                        var tripleColumns = new List<int>();

                        for (int column = 0; column < 9; column++)
                        {
                            var square = squares[row, column];

                            if (square.PossibleNumbers.Contains(firstNumber) &&
                                square.PossibleNumbers.Contains(secondNumber) &&
                                square.PossibleNumbers.Contains(thirdNumber))
                            {
                                tripleColumns.Add(column);
                            }
                        }

                        if (tripleColumns.Count == 3)
                        {
                            // Ensure these numbers do NOT appear in any other square in the row
                            bool onlyInTriple = true;

                            for (int column = 0; column < 9; column++)
                            {
                                if (!tripleColumns.Contains(column))
                                {
                                    var square = squares[row, column];
                                    if (square.PossibleNumbers.Contains(firstNumber) ||
                                        square.PossibleNumbers.Contains(secondNumber) ||
                                        square.PossibleNumbers.Contains(thirdNumber))
                                    {
                                        onlyInTriple = false;
                                        break;
                                    }
                                }
                            }

                            if (onlyInTriple && tripleColumns.All(col => squares[row, col].PossibleNumbers.Count > 3))
                            {
                                solveStep.Explanation =
                                    $"Hidden Triple: Numbers {firstNumber}, {secondNumber} and {thirdNumber} only appear together in three squares in row {row + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                                foreach (var column in tripleColumns)
                                {
                                    solveStep.HighlightedSquares.Add((row, column));

                                    foreach (var num in squares[row, column].PossibleNumbers.ToList())
                                    {
                                        if (num != firstNumber && num != secondNumber && num != thirdNumber)
                                        {
                                            hiddenTripleFound = true;
                                            solveStep.CandidatesRemovedSquares.Add((row, column));
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
        for (int column = 0; column < 9; column++)
        {
            for (int firstNumber = 1; firstNumber <= 7; firstNumber++)
            {
                for (int secondNumber = firstNumber + 1; secondNumber <= 8; secondNumber++)
                {
                    for (int thirdNumber = secondNumber + 1; thirdNumber <= 9; thirdNumber++)
                    {
                        var tripleRows = new List<int>();

                        for (int row = 0; row < 9; row++)
                        {
                            var square = squares[row, column];

                            if (square.PossibleNumbers.Contains(firstNumber) &&
                                square.PossibleNumbers.Contains(secondNumber) &&
                                square.PossibleNumbers.Contains(thirdNumber))
                            {
                                tripleRows.Add(row);
                            }
                        }

                        if (tripleRows.Count == 3)
                        {
                            bool onlyInTriple = true;

                            for (int row = 0; row < 9; row++)
                            {
                                if (!tripleRows.Contains(row))
                                {
                                    var square = squares[row, column];

                                    if (square.PossibleNumbers.Contains(firstNumber) ||
                                        square.PossibleNumbers.Contains(secondNumber) ||
                                        square.PossibleNumbers.Contains(thirdNumber))
                                    {
                                        onlyInTriple = false;
                                        break;
                                    }
                                }
                            }

                            if (onlyInTriple && tripleRows.All(r => squares[r, column].PossibleNumbers.Count > 3))
                            {
                                solveStep.Explanation =
                                    $"Hidden Triple: Numbers {firstNumber}, {secondNumber} and {thirdNumber} only appear together in three squares in column {column + 1}.{Environment.NewLine}Remove other candidates from those squares.";

                                foreach (var row in tripleRows)
                                {
                                    solveStep.HighlightedSquares.Add((row, column));

                                    foreach (var num in squares[row, column].PossibleNumbers.ToList())
                                    {
                                        if (num != firstNumber && num != secondNumber && num != thirdNumber)
                                        {
                                            hiddenTripleFound = true;
                                            solveStep.CandidatesRemovedSquares.Add((row, column));
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
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxColumn = 0; boxColumn < 3; boxColumn++)
            {
                int startRow = boxRow * 3;
                int startColumn = boxColumn * 3;

                for (int firstNumber = 1; firstNumber <= 7; firstNumber++)
                {
                    for (int secondNumber = firstNumber + 1; secondNumber <= 8; secondNumber++)
                    {
                        for (int thirdNumber = secondNumber + 1; thirdNumber <= 9; thirdNumber++)
                        {
                            var tripleCells = new List<(int row, int col)>();

                            for (int r = startRow; r < startRow + 3; r++)
                            {
                                for (int c = startColumn; c < startColumn + 3; c++)
                                {
                                    var square = squares[r, c];
                                    if (square.PossibleNumbers.Contains(firstNumber) &&
                                        square.PossibleNumbers.Contains(secondNumber) &&
                                        square.PossibleNumbers.Contains(thirdNumber))
                                    {
                                        tripleCells.Add((r, c));
                                    }
                                }
                            }

                            if (tripleCells.Count == 3)
                            {
                                bool onlyInTriple = true;

                                for (int r = startRow; r < startRow + 3; r++)
                                {
                                    for (int c = startColumn; c < startColumn + 3; c++)
                                    {
                                        if (!tripleCells.Contains((r, c)))
                                        {
                                            var square = squares[r, c];
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

                                        foreach (var num in squares[cell.row, cell.col].PossibleNumbers.ToList())
                                        {
                                            if (num != firstNumber && num != secondNumber && num != thirdNumber)
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
