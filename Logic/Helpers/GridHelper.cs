using SudokuStepByStep.Models;
using System.Windows;
using System.Windows.Media;

namespace SudokuStepByStep.Logic.Helpers;

public static class GridHelper
{
    public static void UpdatePossibleValues(SudokuSquare[,] squares, bool show)
    {
        int[,] grid = GetNumbers(squares);

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var square = squares[r, c];
                square.PossibleNumbers = RulesHelper.GetPossibleNumbers(grid, r, c);

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
                grid[r, c] = int.TryParse(squares[r, c].Box.Text, out int val) ? val : 0;
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
