using SudokuStepByStep.Models;
using System.Collections.Generic;

namespace SudokuStepByStep.Logic.Helpers;

public static class GridHelper
{
    // Get current numbers as int[,] from the model grid
    public static int[,] GetNumbers(SudokuSquareModel[,] squares)
    {
        int[,] grid = new int[9, 9];

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                grid[r, c] = squares[r, c].Number;
            }
        }

        return grid;
    }

    // Clear numbers for a normal reset (does not affect UI directly)
    public static void ClearSquares(SudokuSquareModel[,] squares)
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                squares[r, c].Number = 0;
                squares[r, c].PossibleNumbers.Clear();
            }
        }
    }

    // Clear numbers for a new puzzle
    public static void ClearSquaresNewPuzzle(SudokuSquareModel[,] squares)
    {
        ClearSquares(squares);
    }

    public static HashSet<int>[,] GetPossibleNumbers(SudokuSquareModel[,] squares)
    {
        var grid = new HashSet<int>[9, 9];

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                // Make a copy of the possible numbers
                grid[row, col] = new HashSet<int>(squares[row, col].PossibleNumbers);
            }
        }

        return grid;
    }
}
