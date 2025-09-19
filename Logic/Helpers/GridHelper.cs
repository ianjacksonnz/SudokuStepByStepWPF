using SudokuStepByStep.Models;
using System.Windows.Media;

namespace SudokuStepByStep.Logic.Helpers;

public static class GridHelper
{
    /// <summary>
    /// Get current numbers as int[,] from the model grid
    /// </summary>
    public static int[,] GetNumbers(SudokuSquare[,] squares)
    {
        int[,] grid = new int[9, 9];

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                grid[rowIndex, columnIndex] = squares[rowIndex, columnIndex].Number;
            }
        }

        return grid;
    }

    /// <summary>
    /// Clear the numbers entered in the grid
    /// </summary>
    public static void ClearSquares(SudokuSquare[,] squares)
    {
        // --- Clear the grid squares for editable squares ---
        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var square = squares[rowIndex, columnIndex];

                if (square.HasNumber && !square.IsReadOnly)
                {
                    square.Number = 0;
                    square.BackgroundColor = Brushes.White;
                }
            }
        }
    }

    /// <summary>
    /// Clear the numbers entered in the grid
    /// </summary>
    public static void ClearSquaresNewPuzzle(SudokuSquare[,] squares)
    {
        // --- Clear the grid squares for editable squares ---
        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                squares[rowIndex, columnIndex].IsReadOnly = false;
                squares[rowIndex, columnIndex].Number = 0;
                squares[rowIndex, columnIndex].BackgroundColor = Brushes.White;
            }
        }
    }

    /// <summary>
    /// Generates a grid of possible numbers for each cell in a Sudoku puzzle.
    /// </summary>
    /// <remarks>The returned grid is a deep copy of the possible numbers from the input grid. Modifying the
    /// returned grid does not affect the original <paramref name="squares"/> grid.</remarks>
    public static HashSet<int>[,] GetPossibleNumbers(SudokuSquare[,] squares)
    {
        var grid = new HashSet<int>[9, 9];

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                // Make a copy of the possible numbers
                grid[rowIndex, columnIndex] = new HashSet<int>(squares[rowIndex, columnIndex].PossibleNumbers);
            }
        }

        return grid;
    }
}
