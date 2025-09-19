using SudokuStepByStep.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuStepByStep.Logic.Helpers;

public static class RulesHelper
{
    // Check if a number can be placed without breaking Sudoku rules
    public static bool IsSafe(SudokuSquare[,] squares, int rowIndex, int columnIndex, int number)
    {
        // Row & Column
        for (int i = 0; i < 9; i++)
        {
            if (squares[rowIndex, i].Number == number) return false;
            if (squares[i, columnIndex].Number == number) return false;
        }

        // 3x3 box
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

    // Set PossibleNumbers for each square
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

    // Get possible numbers for a cell based on current grid
    public static HashSet<int> GetPossibleNumbers(int[,] grid, int rowIndex, int columnIndex)
    {
        var possible = new HashSet<int>();

        for (int number = 1; number <= 9; number++)
        {
            if (NumberNotInRowColumnGrid(grid, rowIndex, columnIndex, number))
            {
                possible.Add(number);
            }
        }

        return possible;
    }

    private static bool NumberNotInRowColumnGrid(int[,] grid, int rowIndex, int columnIndex, int number)
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

    public static bool PuzzleSolved(SudokuSquare[,] squares)
    {
        return !squares.Cast<SudokuSquare>().Any(s => s.Number == 0);
    }

    // Remove a number from possible candidates
    public static void RemovePossibleNumber(SudokuSquare square, int number)
    {
        if (square.PossibleNumbers.Contains(number))
        {
            square.PossibleNumbers.Remove(number);
        }
    }

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
