using SudokuHelper.Common;
using System;
using System.Collections.Generic;

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
        out int colSolved,
        out Enums.CellGroupType? groupType)
    {
        numberSolved = 0;
        rowSolved = -1;
        colSolved = -1;
        groupType = null;

        // Check rows
        for (int row = 0; row < 9; row++)
        {
            for (int num = 1; num <= 9; num++)
            {
                if (RulesHelper.RowHasNumber(board, row, num)) continue;

                var possibleCols = new List<int>();
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col].Count > 0 && board[row, col].Contains(num) && RulesHelper.IsSafe(board, row, col, num))
                        possibleCols.Add(col);
                }

                if (possibleCols.Count == 1)
                {
                    numberSolved = num;
                    rowSolved = row;
                    colSolved = possibleCols[0];
                    groupType = Enums.CellGroupType.Row;
                    return true;
                }
            }
        }

        // Check columns
        for (int col = 0; col < 9; col++)
        {
            for (int num = 1; num <= 9; num++)
            {
                if (RulesHelper.ColHasNumber(board, col, num)) continue;

                var possibleRows = new List<int>();
                for (int row = 0; row < 9; row++)
                {
                    if (board[row, col].Count > 0 && board[row, col].Contains(num) && RulesHelper.IsSafe(board, row, col, num))
                        possibleRows.Add(row);
                }

                if (possibleRows.Count == 1)
                {
                    numberSolved = num;
                    rowSolved = possibleRows[0];
                    colSolved = col;
                    groupType = Enums.CellGroupType.Column;
                    return true;
                }
            }
        }

        // Check 3x3 grids
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                for (int num = 1; num <= 9; num++)
                {
                    if (RulesHelper.GridHasNumber(board, boxRow, boxCol, num)) continue;

                    var possibleCells = new List<(int r, int c)>();
                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (board[r, c].Count > 0 && board[r, c].Contains(num) && RulesHelper.IsSafe(board, r, c, num))
                                possibleCells.Add((r, c));
                        }
                    }

                    if (possibleCells.Count == 1)
                    {
                        numberSolved = num;
                        rowSolved = possibleCells[0].r;
                        colSolved = possibleCells[0].c;
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
    out int numberSolved,
    out int rowSolved,
    out int colSolved,
    out Enums.CellGroupType? groupType)
    {
        numberSolved = 0;
        rowSolved = -1;
        colSolved = -1;
        groupType = null;

        // Check rows for naked pairs
        for (int row = 0; row < 9; row++)
        {
            var candidates = new Dictionary<int, List<int>>();

            for (int col = 0; col < 9; col++)
            {
                if (board[row, col].Count == 2)
                {
                    var possibleNumbers = RulesHelper.GetPossibleNumbers(board, row, col);
                    if (possibleNumbers.Count == 2)
                    {
                        candidates[col] = possibleNumbers;
                    }
                }
            }

            // Find naked pairs
            var nakedPairs = candidates.GroupBy(c => string.Join(",", c.Value)).Where(g => g.Count() == 2);
            foreach (var pair in nakedPairs)
            {
                var cols = pair.Select(c => c.Key).ToList();

                // Remove these candidates from other cells in the same row
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

                // Return the first naked pair as a hint
                numberSolved = pair.First().Value[0]; // pick one of the two candidates
                rowSolved = row;
                colSolved = cols[0];
                groupType = Enums.CellGroupType.Row;
                return true;
            }
        }

        // Optionally, you can add column and grid checks in a similar manner
        // For now, we only implement naked pairs in rows for hints

        return false;
    }


}