namespace SudokuHelper.Common;

public static class RulesHelper
{
    // Change board to store candidate values: each cell is a HashSet<int> (candidates), or null if solved
    // 9x9 grid, each cell: null (solved) or HashSet<int> (candidates)
    public static bool IsSafe(HashSet<int>[,] board, int row, int col, int num)
    {
        // If cell is solved, check for conflicts
        for (int c = 0; c < 9; c++)
        {
            if (board[row, c]?.Count == 1 && board[row, c].Contains(num))
            {
                return false;
            }
        }

        for (int r = 0; r < 9; r++)
        {
            if (board[r, col]?.Count == 1 && board[r, col].Contains(num))
            {
                return false;
            }
        }

        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                if (board[r, c]?.Count == 1 && board[r, c].Contains(num))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool RowHasNumber(HashSet<int>[,] board, int row, int num)
    {
        for (int c = 0; c < 9; c++)
        {
            if (board[row, c]?.Count == 1 && board[row, c].Contains(num))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ColHasNumber(HashSet<int>[,] board, int col, int num)
    {
        for (int r = 0; r < 9; r++)
        {
            if (board[r, col]?.Count == 1 && board[r, col].Contains(num))
            {
                return true;
            }
        }

        return false;
    }

    public static bool GridHasNumber(HashSet<int>[,] board, int boxRow, int boxCol, int num)
    {
        for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
        {
            for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
            {
                if (board[r, c]?.Count == 1 && board[r, c].Contains(num))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static List<int> GetPossibleNumbers(HashSet<int>[,] board, int row, int col)
    {
        var possible = new List<int>();

        if (board[row, col] == null)
        {
            return possible; // solved
        }

        possible.AddRange(board[row, col]);
        return possible;
    }

    public static void RemoveCandidates(HashSet<int>[,] board, int row, int col, string? key)
    {
        // key is a string representation of the candidate list, e.g. "[2, 5]"
        if (string.IsNullOrEmpty(key) || board[row, col] == null || board[row, col].Count == 1)
        {
            return;
        }

        var trimmed = key.Trim('[', ']', ' ');

        foreach (var part in trimmed.Split(','))
        {
            if (int.TryParse(part.Trim(), out int num))
            {
                board[row, col].Remove(num);
            }
        }
    }

    // Helper to set a cell as solved
    public static void SetCell(HashSet<int>[,] board, int row, int col, int value) => board[row, col] = [value];

    // Helper to initialize board with all candidates (1-9) for unsolved cells, or single value for solved
    public static HashSet<int>[,] InitializeBoard(int[,] initial)
    {
        var board = new HashSet<int>[9, 9];

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (initial[r, c] == 0)
                {
                    board[r, c] = [.. Enumerable.Range(1, 9)];
                }
                else
                {
                    board[r, c] = [initial[r, c]];
                }
            }
        }
        return board;
    }
}
