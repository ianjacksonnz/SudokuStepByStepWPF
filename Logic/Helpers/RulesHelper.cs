using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Helpers;

public static class RulesHelper
{
    /// <summary>
    /// Check if a number can be placed without breaking Sudoku rules
    /// </summary>
    public static bool IsSafe(SudokuSquare[,] squares, int rowIndex, int columnIndex, int number)
    {
        // Row & Column
        for (int i = 0; i < 9; i++)
        {
            if (squares[rowIndex, i].Number == number) return false;
            if (squares[i, columnIndex].Number == number) return false;
        }

        // 3x3 Box
        int startRow = rowIndex - rowIndex % 3;
        int startColumn = columnIndex - columnIndex % 3;

        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startColumn; c < startColumn + 3; c++)
            {
                if (squares[r, c].Number == number)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Set PossibleNumbers for each square
    /// </summary>
    /// <param name="squares"></param>
    public static void SetPossibleNumbers(SudokuSquare[,] squares)
    {
        int[,] grid = GridHelper.GetNumbers(squares);

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var square = squares[rowIndex, columnIndex];

                square.PossibleNumbers.Clear();

                if (square.Number == 0)
                {
                    var possibleNumbers = GetPossibleNumbers(grid, rowIndex, columnIndex);

                    foreach (var number in possibleNumbers)
                    {
                        square.PossibleNumbers.Add(number);
                    }
                }
            }
        }
    }

    public static void RemovePossibleNumbersFromGridAfterSquareSolved(SudokuSquare[,] squares, SolveStep solveStep)
    {
        var solvedSquare = squares[solveStep.Row, solveStep.Column];
        solvedSquare.PossibleNumbers.Clear();
        solvedSquare.OnPropertyChanged(nameof(SudokuSquare.PossibleNumbers));

        int startRowIndex = solveStep.Row - solveStep.Row % 3;
        int startColumnIndex = solveStep.Column - solveStep.Column % 3;

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var square = squares[rowIndex, columnIndex];

                bool sameRowIndex = rowIndex == solveStep.Row;
                bool sameColumnIndex = columnIndex == solveStep.Column;
                bool sameBoxIndex = rowIndex >= startRowIndex && rowIndex < startRowIndex + 3 && columnIndex >= startColumnIndex && columnIndex < startColumnIndex + 3;

                if (sameRowIndex || sameColumnIndex || sameBoxIndex)
                {
                    if (square.PossibleNumbers.Contains(solveStep.Number))
                    {
                        square.PossibleNumbers.Remove(solveStep.Number);
                        square.OnPropertyChanged(nameof(SudokuSquare.PossibleNumbers));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get possible numbers for a cell based on current grid
    /// </summary>
    public static HashSet<int> GetPossibleNumbers(int[,] grid, int rowIndex, int columnIndex)
    {
        var possible = new HashSet<int>();

        for (int number = 1; number <= 9; number++)
        {
            if (NumberNotInRowColumnBox(grid, rowIndex, columnIndex, number))
            {
                possible.Add(number);
            }
        }

        return possible;
    }

    /// <summary>
    /// Is the number in the row, colmn or box
    /// </summary>
    private static bool NumberNotInRowColumnBox(int[,] grid, int rowIndex, int columnIndex, int number)
    {
        for (int c = 0; c < 9; c++)
        {
            if (grid[rowIndex, c] == number) return false;
        }

        for (int r = 0; r < 9; r++)
        {
            if (grid[r, columnIndex] == number) return false;
        }

        int startRowIndex = rowIndex - rowIndex % 3;
        int startColumnIndex = columnIndex - columnIndex % 3;

        for (int r = startRowIndex; r < startRowIndex + 3; r++)
        {
            for (int c = startColumnIndex; c < startColumnIndex + 3; c++)
            {
                if (grid[r, c] == number)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Is puzzle solved?
    /// </summary>
    public static bool PuzzleSolved(SudokuSquare[,] squares)
    {
        return !squares.Cast<SudokuSquare>().Any(s => s.Number == 0);
    }

    /// <summary>
    /// Removes a specified number from the list of possible candidates for a Sudoku square.
    /// </summary>
    public static void RemovePossibleNumber(SudokuSquare square, int number)
    {
        if (square.PossibleNumbers.Contains(number))
        {
            square.PossibleNumbers.Remove(number);
        }
    }

    /// <summary>
    /// Removes a number from a list of numbers
    /// </summary>
    public static List<int> RemoveOtherNumbers(params int[] numbersToKeep)
    {
        var result = new List<int>();

        var keepSet = new HashSet<int>(numbersToKeep);

        for (int number = 1; number <= 9; number++)
        {
            if (!keepSet.Contains(number))
            {
                result.Add(number);
            }
        }

        return result;
    }
}
