using SudokuHelper.Common;

namespace SudokuHelper;

public class SolvingRules
{
    /// <summary>
    /// Finds a cell where a number can only go in one place in a row, column, or grid.
    /// Returns true if such a cell is found, with its coordinates, number, and group type.
    /// </summary>
    public static bool OnlyValue(
        HashSet<int>[,] board,
        out int numberSolved,
        out int rowSolved,
        out int columnSolved,
        out Enums.CellGroupType? groupType)
    {
        numberSolved = 0;
        rowSolved = -1;
        columnSolved = -1;
        groupType = null;

        // Check rows
        for (int row = 0; row < 9; row++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleColumns = new List<int>();

                for (int column = 0; column < 9; column++)
                {
                    if (board[row, column].Count > 0 && board[row, column].Contains(number) && RulesHelper.IsSafe(board, row, column, number))
                    {
                        possibleColumns.Add(column);
                    }
                }

                if (possibleColumns.Count == 1)
                {
                    numberSolved = number;
                    rowSolved = row;
                    columnSolved = possibleColumns[0];
                    groupType = Enums.CellGroupType.Row;
                    return true;
                }
            }
        }

        // Check columns
        for (int column = 0; column < 9; column++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleRows = new List<int>();

                for (int row = 0; row < 9; row++)
                {
                    if (board[row, column].Count > 0 && board[row, column].Contains(number) && RulesHelper.IsSafe(board, row, column, number))
                    {
                        possibleRows.Add(row);
                    }
                }

                if (possibleRows.Count == 1)
                {
                    numberSolved = number;
                    rowSolved = possibleRows[0];
                    columnSolved = column;
                    groupType = Enums.CellGroupType.Column;
                    return true;
                }
            }
        }

        // Check 3x3 grids
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxColumn = 0; boxColumn < 3; boxColumn++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    var possibleCells = new List<(int r, int c)>();

                    for (int row = boxRow * 3; row < boxRow * 3 + 3; row++)
                    {
                        for (int column = boxColumn * 3; column < boxColumn * 3 + 3; column++)
                        {
                            if (board[row, column].Count > 0 && board[row, column].Contains(number) && RulesHelper.IsSafe(board, row, column, number))
                            {
                                possibleCells.Add((row, column));
                            }
                        }
                    }

                    if (possibleCells.Count == 1)
                    {
                        numberSolved = number;
                        rowSolved = possibleCells[0].r;
                        columnSolved = possibleCells[0].c;
                        groupType = Enums.CellGroupType.Grid;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool NakedPairs(
        HashSet<int>[,] board,
        List<(int row1, int col1, int row2, int col2)> shownPairs,
        out int[] pairValues,
        out (int row, int col)[] pairCells,
        out Enums.CellGroupType? groupType)
    {
        pairValues = Array.Empty<int>();
        pairCells = Array.Empty<(int, int)>();
        groupType = null;

        // Check rows
        for (int row = 0; row < 9; row++)
        {
            var candidates = new Dictionary<int, List<int>>();

            for (int col = 0; col < 9; col++)
            {
                if (board[row, col].Count == 2)
                {
                    var possibleNumbers = board[row, col].ToList();

                    if (possibleNumbers.Count == 2)
                    {
                        candidates[col] = possibleNumbers;
                    }
                }
            }

            var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value)).Where(g => g.Count() == 2);

            foreach (var pair in nakedPairs)
            {
                var cols = pair.Select(c => c.Key).ToList();
                var candidatePair = (row, cols[0], row, cols[1]);

                if (shownPairs.Contains(candidatePair))
                {
                    continue;
                }

                // Remove candidates from other cells in the same row
                foreach (var col in cols)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        if (c != col && board[row, c].Count > 0)
                        {
                            foreach (var candidate in pair.First().Value)
                            {
                                RulesHelper.RemoveCandidates(board, row, c, candidate.ToString());
                            }
                        }
                    }
                }

                // Return this pair
                pairValues = pair.First().Value.ToArray();
                pairCells = new[] { (row, cols[0]), (row, cols[1]) };
                groupType = Enums.CellGroupType.Row;
                shownPairs.Add(candidatePair);

                return true;
            }
        }

        // --- Check Columns ---
        for (int col = 0; col < 9; col++)
        {
            var candidates = new Dictionary<int, List<int>>();

            for (int row = 0; row < 9; row++)
            {
                if (board[row, col].Count == 2)
                {
                    var possibleNumbers = board[row, col].ToList();
                    candidates[row] = possibleNumbers;
                }
            }

            var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value)).Where(g => g.Count() == 2);

            foreach (var pair in nakedPairs)
            {
                var rows = pair.Select(c => c.Key).ToList();
                var candidatePair = (rows[0], col, rows[1], col);

                if (shownPairs.Contains(candidatePair))
                {
                    continue;
                }

                // Remove candidates from other cells in the column
                foreach (var row in rows)
                {
                    for (int r = 0; r < 9; r++)
                    {
                        if (r != row && board[r, col].Count > 0)
                        {
                            foreach (var candidate in pair.First().Value)
                            {
                                RulesHelper.RemoveCandidates(board, r, col, candidate.ToString());
                            }
                        }
                    }
                }

                pairValues = pair.First().Value.ToArray();
                pairCells = new[] { (rows[0], col), (rows[1], col) };
                groupType = Enums.CellGroupType.Column;
                shownPairs.Add(candidatePair);
                return true;
            }
        }

        // --- Check 3x3 Grids ---
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                var candidates = new Dictionary<(int, int), List<int>>();

                for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                {
                    for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                    {
                        if (board[r, c].Count == 2)
                        {
                            candidates[(r, c)] = board[r, c].ToList();
                        }
                    }
                }

                var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value)).Where(g => g.Count() == 2);

                foreach (var pair in nakedPairs)
                {
                    var cells = pair.Select(c => c.Key).ToList();
                    var candidatePair = (cells[0].Item1, cells[0].Item2, cells[1].Item1, cells[1].Item2);

                    if (shownPairs.Contains(candidatePair))
                    {
                        continue;
                    }

                    // Remove candidates from other cells in the grid
                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (!cells.Contains((r, c)) && board[r, c].Count > 0)
                            {
                                foreach (var candidate in pair.First().Value)
                                {
                                    RulesHelper.RemoveCandidates(board, r, c, candidate.ToString());
                                }
                            }
                        }
                    }

                    pairValues = pair.First().Value.ToArray();
                    pairCells = new[] { cells[0], cells[1] };
                    groupType = Enums.CellGroupType.Grid;
                    shownPairs.Add(candidatePair);
                    return true;
                }
            }
        }

        return false;
    }


}