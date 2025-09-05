using SudokuStepByStep.Logic.Helpers;

namespace SudokuStepByStep.Logic;

public class SudokuSolver
{
    public static bool Solve(int[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (GridHelper.IsSafe(grid, row, col, num))
                        {
                            grid[row, col] = num;

                            if (Solve(grid))
                            {
                                return true;
                            }

                            grid[row, col] = 0; // backtrack
                        }
                    }

                    return false; // no valid number found
                }
            }
        }

        return true; // solved
    }
}
