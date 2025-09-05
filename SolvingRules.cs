using SudokoStepByStep.Common;

namespace SudokoStepByStep;

public class SolvingRules
{
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

        // --- rows ---
        for (int row = 0; row < 9; row++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleColumns = new List<int>();
                for (int column = 0; column < 9; column++)
                    if (board[row, column].Count > 0 &&
                        board[row, column].Contains(number) &&
                        RulesHelper.IsSafe(board, row, column, number))
                        possibleColumns.Add(column);

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

        // --- columns ---
        for (int column = 0; column < 9; column++)
        {
            for (int number = 1; number <= 9; number++)
            {
                var possibleRows = new List<int>();

                for (int row = 0; row < 9; row++)
                {
                    if (board[row, column].Count > 0 &&
                        board[row, column].Contains(number) &&
                        RulesHelper.IsSafe(board, row, column, number))
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

        // --- grids ---
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    var possibleCells = new List<(int r, int c)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (board[r, c].Count > 0 &&
                                board[r, c].Contains(number) &&
                                RulesHelper.IsSafe(board, r, c, number))
                            {
                                possibleCells.Add((r, c));
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
        List<HashSet<(int row, int col)>> shownGroups,
        out int[] pairValues,
        out (int row, int col)[] pairCells,
        out Enums.CellGroupType? groupType)
    {
        pairValues = Array.Empty<int>();
        pairCells = Array.Empty<(int, int)>();
        groupType = null;

        // --- Rows ---
        for (int row = 0; row < 9; row++)
        {
            var candidates = new Dictionary<int, List<int>>();
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col].Count == 2)
                {
                    candidates[col] = board[row, col].ToList();
                }
            }

            var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value))
                                       .Where(g => g.Count() == 2);

            foreach (var pair in nakedPairs)
            {
                var cols = pair.Select(c => c.Key).ToList();
                var groupCells = new HashSet<(int, int)> { (row, cols[0]), (row, cols[1]) };

                // skip if already hinted
                if (shownGroups.Any(g => g.SetEquals(groupCells)))
                {
                    continue;
                }

                bool removed = false;

                // Remove these two candidates from other cells in the row
                foreach (int c in Enumerable.Range(0, 9))
                {
                    if (cols.Contains(c))
                        continue;

                    foreach (var val in pair.First().Value)
                    {
                        if (board[row, c].Contains(val))
                        {
                            RulesHelper.RemoveCandidates(board, row, c, val.ToString());
                            removed = true;
                        }
                    }
                }

                if (removed)
                {
                    pairValues = pair.First().Value.ToArray();
                    pairCells = groupCells.ToArray();
                    groupType = Enums.CellGroupType.Row;
                    return true; // return because we show one hint at a time
                }
            }
        }

        // --- Columns ---
        for (int col = 0; col < 9; col++)
        {
            var candidates = new Dictionary<int, List<int>>();
            for (int row = 0; row < 9; row++)
            {
                if (board[row, col].Count == 2)
                {
                    candidates[row] = board[row, col].ToList();
                }
            }

            var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value))
                                       .Where(g => g.Count() == 2);

            foreach (var pair in nakedPairs)
            {
                var rows = pair.Select(c => c.Key).ToList();
                var groupCells = new HashSet<(int, int)> { (rows[0], col), (rows[1], col) };

                if (shownGroups.Any(g => g.SetEquals(groupCells)))
                {
                    continue;
                }

                bool removed = false;

                // Remove these two candidates from other cells in the column
                for (int r = 0; r < 9; r++)
                {
                    if (rows.Contains(r))
                        continue;

                    foreach (var val in pair.First().Value)
                    {
                        if (board[r, col].Contains(val))
                        {
                            RulesHelper.RemoveCandidates(board, r, col, val.ToString());
                            removed = true;
                        }
                    }
                }

                if (removed)
                {
                    pairValues = pair.First().Value.ToArray();
                    pairCells = groupCells.ToArray();
                    groupType = Enums.CellGroupType.Column;
                    return true;
                }
            }
        }

        // --- Grids ---
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

                var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value))
                                          .Where(g => g.Count() == 2);

                foreach (var pair in nakedPairs)
                {
                    var cells = new HashSet<(int, int)>(pair.Select(c => c.Key));

                    if (shownGroups.Any(g => g.SetEquals(cells)))
                        continue;

                    bool removed = false;

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (cells.Contains((r, c)))
                                continue;

                            foreach (var val in pair.First().Value)
                            {
                                if (board[r, c].Contains(val))
                                {
                                    RulesHelper.RemoveCandidates(board, r, c, val.ToString());
                                    removed = true;
                                }
                            }
                        }
                    }

                    if (removed)
                    {
                        pairValues = pair.First().Value.ToArray();
                        pairCells = cells.ToArray();
                        groupType = Enums.CellGroupType.Grid;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool ApplyPointingPairs(
        HashSet<int>[,] board,
        List<HashSet<(int row, int col)>> hintedGroups,
        out int[] pairValues,
        out (int row, int col)[] pairCells,
        out Enums.CellGroupType? groupType)
    {
        pairValues = Array.Empty<int>();
        pairCells = Array.Empty<(int, int)>();
        groupType = null;

        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    var candidateCells = new List<(int r, int c)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            
                            if (board[r, c].Contains(number))
                            {
                                candidateCells.Add((r, c));
                            }
                        }
                    }

                    if (candidateCells.Count <= 1)
                        continue;

                    // Check same row
                    if (candidateCells.All(cell => cell.r == candidateCells[0].r))
                    {
                        int row = candidateCells[0].r;
                        bool removed = false;

                        for (int c = 0; c < 9; c++)
                        {
                            if (c < boxCol * 3 || c >= boxCol * 3 + 3)
                            {
                                if (board[row, c].Contains(number))
                                {
                                    RulesHelper.RemoveCandidates(board, row, c, number.ToString());
                                    removed = true;
                                }
                            }
                        }

                        if (removed)
                        {
                            pairValues = new int[] { number };
                            pairCells = candidateCells.ToArray();
                            groupType = Enums.CellGroupType.Row;
                            return true; // return immediately for first hint
                        }
                    }

                    // Check same column
                    if (candidateCells.All(cell => cell.c == candidateCells[0].c))
                    {
                        int col = candidateCells[0].c;
                        bool removed = false;

                        for (int r = 0; r < 9; r++)
                        {
                            if (r < boxRow * 3 || r >= boxRow * 3 + 3)
                            {
                                if (board[r, col].Contains(number))
                                {
                                    RulesHelper.RemoveCandidates(board, r, col, number.ToString());
                                    removed = true;
                                }
                            }
                        }

                        if (removed)
                        {
                            pairValues = new int[] { number };
                            pairCells = candidateCells.ToArray();
                            groupType = Enums.CellGroupType.Column;
                            return true; // return immediately for first hint
                        }
                    }
                }
            }
        }

        return false;
    }
}
