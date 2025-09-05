namespace SudokoStepByStep.Common;

public static class RulesHelper
{
    public static bool IsSafe(HashSet<int>[,] board, int row, int column, int number)
    {
        // Check row
        for (int c = 0; c < 9; c++)
        {
            // Only treat as conflict if the cell is solved (null or Count == 0) or already entered
            if (c != column && board[row, c] == null)
            {
                continue; // solved cell is fine unless it matches the number
            }

            if (board[row, c]?.Count == 0)
            {
                continue; // empty candidate set means solved
            }

            if (board[row, c]?.Count == 1 && board[row, c].Contains(number) && board[row, c] != board[row, column])
            {
                return false;
            }
        }

        // Check column
        for (int r = 0; r < 9; r++)
        {
            if (r != row && board[r, column] == null)
            {
                continue;
            }

            if (board[r, column]?.Count == 0)
            {
                continue;
            }

            if (board[r, column]?.Count == 1 && board[r, column].Contains(number) && board[r, column] != board[row, column])
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
                if ((r != row || c != column) && board[r, c] == null) continue;

                if (board[r, c]?.Count == 0) continue;

                if (board[r, c]?.Count == 1 && board[r, c].Contains(number) && board[r, c] != board[row, column])
                {
                    return false;
                }
            }
        }

        return true;
    }


    public static bool NumberOnlyInOnePlaceOnRow(HashSet<int>[,] board, int row, int number)
    {
        for (int column = 0; column < 9; column++)
        {
            if (board[row, column]?.Count == 1 && board[row, column].Contains(number))
            {
                return true;
            }
        }

        return false;
    }

    public static bool NumberOnlyInOnePlaceOnColumn(HashSet<int>[,] board, int column, int number)
    {
        for (int row = 0; row < 9; row++)
        {
            if (board[row, column]?.Count == 1 && board[row, column].Contains(number))
            {
                return true;
            }
        }

        return false;
    }

    public static bool NumberOnlyInOnePlaceOnGrid(HashSet<int>[,] board, int boxRow, int boxColumn, int number)
    {
        for (int row = boxRow * 3; row < boxRow * 3 + 3; row++)
        {
            for (int column = boxColumn * 3; column < boxColumn * 3 + 3; column++)
            {
                if (board[row, column]?.Count == 1 && board[row, column].Contains(number))
                {
                    
                    return true;
                }
            }
        }

        return false;
    }

    public static List<int> GetPossibleNumbers(HashSet<int>[,] board, int row, int column)
    {
        var possible = new List<int>();

        if (board[row, column] == null)
        {
            return possible; // solved
        }

        possible.AddRange(board[row, column]);
        return possible;
    }

    public static void RemoveCandidates(HashSet<int>[,] board, int row, int column, string? key)
    {
        // key is a string representation of the candidate list, e.g. "[2, 5]"
        if (string.IsNullOrEmpty(key) || board[row, column] == null || board[row, column].Count == 1)
        {
            return;
        }

        var trimmed = key.Trim('[', ']', ' ');

        foreach (var part in trimmed.Split(','))
        {
            if (int.TryParse(part.Trim(), out int num))
            {
                board[row, column].Remove(num);
            }
        }
    }

    // Helper to set a cell as solved
    public static void SetCell(HashSet<int>[,] board, int row, int column, int value) => board[row, column] = [value];

    // Helper to initialize board with all candidates (1-9) for unsolved cells, or single value for solved
    public static HashSet<int>[,] InitializeBoard(int[,] initial)
    {
        var board = new HashSet<int>[9, 9];

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (initial[row, column] == 0)
                {
                    board[row, column] = [.. Enumerable.Range(1, 9)];
                }
                else
                {
                    board[row, column] = [initial[row, column]];
                }
            }
        }
        return board;
    }
}
