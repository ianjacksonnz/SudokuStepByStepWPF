namespace SudokoStepByStep;

public class SudokuSolver
{
    public static bool Solve(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsSafe(board, row, col, num))
                        {
                            board[row, col] = num;

                            if (Solve(board))
                            {
                                return true;
                            }

                            board[row, col] = 0; // backtrack
                        }
                    }

                    return false; // no valid number found
                }
            }
        }

        return true; // solved
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
}
