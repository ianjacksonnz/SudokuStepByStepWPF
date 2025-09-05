using System.Windows.Media;

namespace SudokuStepByStep.Logic.Helpers;

public static class GridHelper
{
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
    /// Check if it's safe to place a number in a given cell
    /// </summary>
    /// <param name="board"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="num"></param>
    /// <returns></returns>
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

    public static List<int> GetPossibleNumbers(int[,] board, int row, int col)
    {
        List<int> possible = [];

        for (int num = 1; num <= 9; num++)
        {
            if (IsSafe(board, row, col, num))
            {
                possible.Add(num);
            }
        }

        return possible;
    }

    /// <summary>
    /// Clear the numbers enetered in the grid
    /// </summary>
    /// <param name="squares"></param>
    public static void ClearSquares(SudokuSquare[,] squares)
    {
        // --- Clear the board squares for editable squares ---
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
}
