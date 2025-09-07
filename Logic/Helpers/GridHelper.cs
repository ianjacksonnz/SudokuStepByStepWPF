using SudokuStepByStep.Models;
using System.Windows;
using System.Windows.Media;

namespace SudokuStepByStep.Logic.Helpers;

public static class GridHelper
{
    public static HashSet<int>[,] GetPossibleNumbers(SudokuSquare[,] squares)
    {
        var grid = new HashSet<int>[9, 9];

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                grid[row, col] = squares[row, col].PossibleNumbers;
            }
        }

        return grid;
    }

    public static void SetPossibleValues(SudokuSquare[,] squares, bool show)
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

    /// <summary>
    /// Clear the numbers entered in the grid
    /// </summary>
    /// <param name="squares"></param>
    public static void ClearSquares(SudokuSquare[,] squares)
    {
        // --- Clear the grid squares for editable squares ---
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (!squares[r, c].Box.IsReadOnly)
                {
                    squares[r, c].Number = 0;
                    squares[r, c].Box.Text = "";
                    squares[r, c].Box.Background = Brushes.White; // reset background highlighting
                }
            }
        }
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
}
