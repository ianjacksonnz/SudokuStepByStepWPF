using SudokuStepByStep.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuStepByStep.Logic.Helpers;

public static class RulesHelper
{
    // Check if a number can be placed without breaking Sudoku rules
    public static bool IsSafe(SudokuSquare[,] squares, int row, int col, int number)
    {
        // Row & Column
        for (int i = 0; i < 9; i++)
        {
            if (squares[row, i].Number == number) return false;
            if (squares[i, col].Number == number) return false;
        }

        // 3x3 box
        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int r = startRow; r < startRow + 3; r++)
            for (int c = startCol; c < startCol + 3; c++)
                if (squares[r, c].Number == number) return false;

        return true;
    }

    // Set PossibleNumbers for each square
    public static void SetPossibleNumbers(SudokuSquare[,] squares)
    {
        int[,] grid = GridHelper.GetNumbers(squares);

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var square = squares[r, c];

                square.PossibleNumbers.Clear();

                if (square.Number == 0)
                {
                    foreach (var num in GetPossibleNumbers(grid, r, c))
                    {
                        square.PossibleNumbers.Add(num);
                    }
                }
            }
        }
    }

    // Get possible numbers for a cell based on current grid
    public static HashSet<int> GetPossibleNumbers(int[,] grid, int row, int col)
    {
        var possible = new HashSet<int>();

        for (int num = 1; num <= 9; num++)
        {
            if (NumberNotInRowColumnGrid(grid, row, col, num))
                possible.Add(num);
        }

        return possible;
    }

    private static bool NumberNotInRowColumnGrid(int[,] grid, int row, int col, int number)
    {
        for (int c = 0; c < 9; c++) if (grid[row, c] == number) return false;
        for (int r = 0; r < 9; r++) if (grid[r, col] == number) return false;

        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int r = startRow; r < startRow + 3; r++)
            for (int c = startCol; c < startCol + 3; c++)
                if (grid[r, c] == number) return false;

        return true;
    }

    public static bool PuzzleSolved(SudokuSquare[,] squares)
    {
        return !squares.Cast<SudokuSquare>().Any(s => s.Number == 0);
    }

    // Remove a number from possible candidates
    public static void RemovePossibleNumber(SudokuSquare square, int number)
    {
        if (square.PossibleNumbers.Contains(number))
            square.PossibleNumbers.Remove(number);
    }

    public static List<int> RemoveOtherNumbers(params int[] numbersToKeep)
    {
        var result = new List<int>();

        var keepSet = new HashSet<int>(numbersToKeep);

        for (int num = 1; num <= 9; num++)
        {
            if (!keepSet.Contains(num))
            {
                result.Add(num);
            }
        }

        return result;
    }

}
