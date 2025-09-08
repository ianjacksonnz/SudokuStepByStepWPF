using SudokuStepByStep.Models;
using System.Windows;

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

    public static void SetPossibleNumbers(SudokuSquare[,] squares, bool show)
    {
        int[,] grid = GetNumbers(squares);

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                var square = squares[row, column];

                if (squares[row, column].Number == 0)
                {
                    square.PossibleNumbers = RulesHelper.GetPossibleNumbers(grid, row, column);
                }
                else
                {
                    square.PossibleNumbers = new HashSet<int>();
                }

                ShowPossibleValues(squares, show);
            }
        }
    }

    public static void RemovePossibleNumbersFromGridAfterSolvedSquare(SudokuSquare[,] squares, SolveStep solveStep)
    {
        int startRow = solveStep.Row - solveStep.Row % 3;
        int startCol = solveStep.Column - solveStep.Column % 3;

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                var square = squares[row, column];

                bool sameRow = row == solveStep.Row;
                bool sameCol = column == solveStep.Column;
                bool sameBox = row >= startRow && row < startRow + 3 && column >= startCol && column < startCol + 3;

                if (sameRow || sameCol || sameBox)
                {
                    if (square.PossibleNumbers.Contains(solveStep.Number))
                    {
                        square.PossibleNumbers.Remove(solveStep.Number);
                    }
                }

                ShowPossibleValues(squares, true);
            }
        }
    }

    public static void RemovePossibleNumbersFromSquare(SudokuSquare[,] squares, SudokuSquare square, int number)
    {
        if (square.PossibleNumbers.Contains(number))
        {
            square.PossibleNumbers.Remove(number);
        }

        ShowPossibleValues(squares, true);
    }

    public static void ShowPossibleValues(SudokuSquare[,] squares, bool show)
    {
        int[,] grid = GetNumbers(squares);

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                var square = squares[row, column];

                if (show)
                {
                    square.CandidatesBlock.Text = string.Join(" ", square.PossibleNumbers);
                    square.CandidatesBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    square.CandidatesBlock.Text = string.Empty;
                    square.CandidatesBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    public static int[,] GetNumbers(SudokuSquare[,] squares)
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

    public static HashSet<int> GetPossibleNumbers(int[,] grid, int row, int col)
    {
        var possible = new HashSet<int>();

        for (int num = 1; num <= 9; num++)
        {
            if (NumberNotInRowColumnGrid(grid, row, col, num))
            {
                possible.Add(num);
            }
        }

        return possible;
    }

    /// <summary>
    /// Checks if a number can be placed in a given cell without violating rules
    /// that the number is not in the same row, column or grid
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static bool NumberNotInRowColumnGrid(int[,] grid, int row, int col, int number)
    {
        // Check row
        for (int c = 0; c < 9; c++)
        {
            if (grid[row, c] == number)
            {
                return false;
            }
        }

        // Check column
        for (int r = 0; r < 9; r++)
        {
            if (grid[r, col] == number)
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
                if (grid[r, c] == number)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a number can be placed in a given cell using PossibleNumbers 
    /// </summary>
    /// <param name="gridPossibleNumbers"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static bool IsPossibleNumber(HashSet<int>[,] gridPossibleNumbers, int row, int column, int number)
    {
        // Check row
        for (int c = 0; c < 9; c++)
        {
            if (c != column && gridPossibleNumbers[row, c]?.Count == 1 && gridPossibleNumbers[row, c].Contains(number))
            {
                return false;
            }
        }

        // Check column
        for (int r = 0; r < 9; r++)
        {
            if (r != row && gridPossibleNumbers[r, column]?.Count == 1 && gridPossibleNumbers[r, column].Contains(number))
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
                if ((r != row || c != column) && gridPossibleNumbers[r, c]?.Count == 1 && gridPossibleNumbers[r, c].Contains(number))
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
                        if (IsPossibleNumber(grid, row, col, num))
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

    public static bool PuzzleSolved(SudokuSquare[,] squares)
    {
        int[,] grid = GetNumbers(squares);

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }


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
