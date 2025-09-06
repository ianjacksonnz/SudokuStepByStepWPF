using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Helpers;

public static class RulesHelper
{
    public static void RemoveCandidates(HashSet<int>[,] grid, int row, int column, string? key)
    {
        // key is a string representation of the candidate list, e.g. "[2, 5]"
        if (string.IsNullOrEmpty(key) || grid[row, column] == null || grid[row, column].Count == 1)
        {
            return;
        }

        var trimmed = key.Trim('[', ']', ' ');

        foreach (var part in trimmed.Split(','))
        {
            if (int.TryParse(part.Trim(), out int num))
            {
                grid[row, column].Remove(num);
            }
        }
    }

    public static List<int> GetPossibleNumbers(HashSet<int>[,] grid, int row, int col)
    {
        var possible = new List<int>();

        for (int num = 1; num <= 9; num++)
        {
            if (IsSafe(grid, row, col, num))
            {
                possible.Add(num);
            }
        }

        return possible;
    }

    public static HashSet<int> GetPossibleNumbers(int[,] grid, int row, int col)
    {
        var possible = new HashSet<int>();

        for (int num = 1; num <= 9; num++)
        {
            if (IsSafe(grid, row, col, num))
            {
                possible.Add(num);
            }
        }

        return possible;
    }

    public static bool IsSafe(HashSet<int>[,] grid, int row, int column, int number)
    {
        // Check row
        for (int c = 0; c < 9; c++)
        {
            if (c != column && grid[row, c]?.Count == 1 && grid[row, c].Contains(number))
            {
                return false;
            }
        }

        // Check column
        for (int r = 0; r < 9; r++)
        {
            if (r != row && grid[r, column]?.Count == 1 && grid[r, column].Contains(number))
            {
                return false;
            }
        }

        // Check 3x3 box
        int startRow = row - row % 3;
        int startCol = column - column % 3;

        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                if ((r != row || c != column) && grid[r, c]?.Count == 1 && grid[r, c].Contains(number))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool IsSafe(int[,] board, int row, int col, int num)
    {
        // Check row
        for (int c = 0; c < 9; c++)
        {
            if (board[row, c] == num)
            {
                return false;
            }
        }

        // Check column
        for (int r = 0; r < 9; r++)
        {
            if (board[r, col] == num)
            {
                return false;
            }
        }

        // Check 3x3 box
        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                if (board[r, c] == num)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool SolveCompletePuzzle(HashSet<int>[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == null || grid[row, col].Count == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsSafe(grid, row, col, num))
                        {
                            grid[row, col] = new HashSet<int> { num };

                            if (SolveCompletePuzzle(grid))
                            {
                                return true;
                            }

                            grid[row, col] = null; // backtrack
                        }
                    }

                    return false; // no valid number found
                }
            }
        }

        return true; // solved
    }

    //public static bool NumberOnlyInOnePlaceOnRow(HashSet<int>[,] grid, int row, int number)
    //{
    //    for (int column = 0; column < 9; column++)
    //    {
    //        if (grid[row, column]?.Count == 1 && grid[row, column].Contains(number))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //public static bool NumberOnlyInOnePlaceOnColumn(HashSet<int>[,] grid, int column, int number)
    //{
    //    for (int row = 0; row < 9; row++)
    //    {
    //        if (grid[row, column]?.Count == 1 && grid[row, column].Contains(number))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //public static bool NumberOnlyInOnePlaceOnGrid(HashSet<int>[,] grid, int boxRow, int boxColumn, int number)
    //{
    //    for (int row = boxRow * 3; row < boxRow * 3 + 3; row++)
    //    {
    //        for (int column = boxColumn * 3; column < boxColumn * 3 + 3; column++)
    //        {
    //            if (grid[row, column]?.Count == 1 && grid[row, column].Contains(number))
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}


    //// Helper to set a square as solved
    //public static void SetSquare(HashSet<int>[,] grid, int row, int column, int value)
    //{
    //    grid[row, column] = new HashSet<int> { value };
    //}

    //// Helper to initialize grid with all candidates (1-9) for unsolved squares, or single value for solved
    //public static HashSet<int>[,] InitializeBoard(int[,] initial)
    //{
    //    var grid = new HashSet<int>[9, 9];

    //    for (int row = 0; row < 9; row++)
    //    {
    //        for (int column = 0; column < 9; column++)
    //        {
    //            if (initial[row, column] == 0)
    //            {
    //                grid[row, column] = new HashSet<int>(Enumerable.Range(1, 9));
    //            }
    //            else
    //            {
    //                grid[row, column] = new HashSet<int> { initial[row, column] };
    //            }
    //        }
    //    }
    //    return grid;
    //}

    // Helper to set a cell as solved
    // public static void SetCell(HashSet<int>[,] board, int row, int column, int value) => board[row, column] = [value];
}
