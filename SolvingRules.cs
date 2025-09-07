using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;

namespace SudokuStepByStep;

public class SolvingRules
{
    public static bool OnlyValue(
        HashSet<int>[,] board,
        out int numberSolved,
        out int rowSolved,
        out int columnSolved,
        out Enums.SquareGroupType? groupType)
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
                        RulesHelper.IsPossibleNumber(board, row, column, number))
                        possibleColumns.Add(column);

                if (possibleColumns.Count == 1)
                {
                    numberSolved = number;
                    rowSolved = row;
                    columnSolved = possibleColumns[0];
                    groupType = Enums.SquareGroupType.Row;
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
                        RulesHelper.IsPossibleNumber(board, row, column, number))
                    {
                        possibleRows.Add(row);
                    }
                }

                if (possibleRows.Count == 1)
                {
                    numberSolved = number;
                    rowSolved = possibleRows[0];
                    columnSolved = column;
                    groupType = Enums.SquareGroupType.Column;
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
                    var possibleSquares = new List<(int r, int c)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (board[r, c].Count > 0 &&
                                board[r, c].Contains(number) &&
                                RulesHelper.IsPossibleNumber(board, r, c, number))
                            {
                                possibleSquares.Add((r, c));
                            }
                        }
                    }

                    if (possibleSquares.Count == 1)
                    {
                        numberSolved = number;
                        rowSolved = possibleSquares[0].r;
                        columnSolved = possibleSquares[0].c;
                        groupType = Enums.SquareGroupType.Grid;
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
        out (int row, int col)[] pairSquares,
        out Enums.SquareGroupType? groupType)
    {
        pairValues = Array.Empty<int>();
        pairSquares = Array.Empty<(int, int)>();
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
                var groupSquares = new HashSet<(int, int)> { (row, cols[0]), (row, cols[1]) };

                // skip if already hinted
                if (shownGroups.Any(g => g.SetEquals(groupSquares)))
                {
                    continue;
                }

                bool removed = false;

                // Remove these two candidates from other squares in the row
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
                    pairSquares = groupSquares.ToArray();
                    groupType = Enums.SquareGroupType.Row;
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
                var groupSquares = new HashSet<(int, int)> { (rows[0], col), (rows[1], col) };

                if (shownGroups.Any(g => g.SetEquals(groupSquares)))
                {
                    continue;
                }

                bool removed = false;

                // Remove these two candidates from other squares in the column
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
                    pairSquares = groupSquares.ToArray();
                    groupType = Enums.SquareGroupType.Column;
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
                    var squares = new HashSet<(int, int)>(pair.Select(c => c.Key));

                    if (shownGroups.Any(g => g.SetEquals(squares)))
                        continue;

                    bool removed = false;

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (squares.Contains((r, c)))
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
                        pairSquares = squares.ToArray();
                        groupType = Enums.SquareGroupType.Grid;
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
        out (int row, int col)[] pairSquares,
        out Enums.SquareGroupType? groupType)
    {
        pairValues = Array.Empty<int>();
        pairSquares = Array.Empty<(int, int)>();
        groupType = null;

        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    var candidateSquares = new List<(int r, int c)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            
                            if (board[r, c].Contains(number))
                            {
                                candidateSquares.Add((r, c));
                            }
                        }
                    }

                    if (candidateSquares.Count <= 1)
                        continue;

                    // Check same row
                    if (candidateSquares.All(square => square.r == candidateSquares[0].r))
                    {
                        int row = candidateSquares[0].r;
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
                            pairSquares = candidateSquares.ToArray();
                            groupType = Enums.SquareGroupType.Row;
                            return true; // return immediately for first hint
                        }
                    }

                    // Check same column
                    if (candidateSquares.All(square => square.c == candidateSquares[0].c))
                    {
                        int col = candidateSquares[0].c;
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
                            pairSquares = candidateSquares.ToArray();
                            groupType = Enums.SquareGroupType.Column;
                            return true; // return immediately for first hint
                        }
                    }
                }
            }
        }

        return false;
    }
}
