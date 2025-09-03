using System;
using System.Collections.Generic;

namespace SudokuHelper
{
    public class SolvingRules
    {
        /// <summary>
        /// Finds a cell in the board where a number can only go in one place in a row, column, or grid.
        /// Returns true if such a cell is found, with its coordinates, number, and group type.
        /// </summary>
        public static bool OnlyValue(int[,] board, out int numberSolved, out int rowSolved, out int colSolved, out string groupType)
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
                    if (RowHasNumber(board, row, num)) continue;
                    var possibleCols = new List<int>();
                    for (int col = 0; col < 9; col++)
                    {
                        if (board[row, col] == 0 && IsSafe(board, row, col, num))
                            possibleCols.Add(col);
                    }
                    if (possibleCols.Count == 1)
                    {
                        numberSolved = num;
                        rowSolved = row;
                        colSolved = possibleCols[0];
                        groupType = "row";
                        return true;
                    }
                }
            }

            // Check columns
            for (int col = 0; col < 9; col++)
            {
                for (int num = 1; num <= 9; num++)
                {
                    if (ColHasNumber(board, col, num)) continue;
                    var possibleRows = new List<int>();
                    for (int row = 0; row < 9; row++)
                    {
                        if (board[row, col] == 0 && IsSafe(board, row, col, num))
                            possibleRows.Add(row);
                    }
                    if (possibleRows.Count == 1)
                    {
                        numberSolved = num;
                        rowSolved = possibleRows[0];
                        colSolved = col;
                        groupType = "column";
                        return true;
                    }
                }
            }

            // Check grids
            for (int boxRow = 0; boxRow < 3; boxRow++)
            {
                for (int boxCol = 0; boxCol < 3; boxCol++)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (GridHasNumber(board, boxRow, boxCol, num)) continue;
                        var possibleCells = new List<(int r, int c)>();
                        for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                        {
                            for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                            {
                                if (board[r, c] == 0 && IsSafe(board, r, c, num))
                                    possibleCells.Add((r, c));
                            }
                        }
                        if (possibleCells.Count == 1)
                        {
                            numberSolved = num;
                            rowSolved = possibleCells[0].r;
                            colSolved = possibleCells[0].c;
                            groupType = "grid";
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsSafe(int[,] board, int row, int col, int num)
        {
            for (int c = 0; c < 9; c++)
                if (board[row, c] == num) return false;
            for (int r = 0; r < 9; r++)
                if (board[r, col] == num) return false;
            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int r = startRow; r < startRow + 3; r++)
                for (int c = startCol; c < startCol + 3; c++)
                    if (board[r, c] == num) return false;
            return true;
        }
        private static bool RowHasNumber(int[,] board, int row, int num)
        {
            for (int c = 0; c < 9; c++)
                if (board[row, c] == num) return true;
            return false;
        }
        private static bool ColHasNumber(int[,] board, int col, int num)
        {
            for (int r = 0; r < 9; r++)
                if (board[r, col] == num) return true;
            return false;
        }
        private static bool GridHasNumber(int[,] board, int boxRow, int boxCol, int num)
        {
            for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                    if (board[r, c] == num) return true;
            return false;
        }
    }
}
