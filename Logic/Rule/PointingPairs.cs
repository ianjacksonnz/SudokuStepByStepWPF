using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule
{
    /// <summary>
    /// If a number is present in only two sqaures of a box, then it must be the solution for one of these two squares. 
    /// If these two squares belong to the same row or column, then this candidate can not be the solution in any other square of the same row or column, respectively.
    /// </summary>
    public static class PointingPairs
    {
        public static SolveStep Run(SudokuSquare[,] squares)
        {
            var solveStep = new SolveStep()
            {
                Rule = Enums.SolvingRule.PointingPairs,
                HighlightedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedSquares = new HashSet<(int row, int col)>()
            };

            // Precompute candidate positions for each number in boxes, rows, and columns
            var boxCandidates = new List<(int row, int col)>[9, 9]; // boxIndex, number-1
            var rowCandidates = new List<(int row, int col)>[9, 9];
            var colCandidates = new List<(int row, int col)>[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int n = 0; n < 9; n++)
                {
                    boxCandidates[i, n] = new List<(int row, int col)>();
                    rowCandidates[i, n] = new List<(int row, int col)>();
                    colCandidates[i, n] = new List<(int row, int col)>();
                }
            }

            for (int rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    foreach (var number in squares[rowIndex, columnIndex].PossibleNumbers)
                    {
                        int numberIndex = number - 1;
                        int boxIndex = (rowIndex / 3) * 3 + (columnIndex / 3);

                        boxCandidates[boxIndex, numberIndex].Add((rowIndex, columnIndex));
                        rowCandidates[rowIndex, numberIndex].Add((rowIndex, columnIndex));
                        colCandidates[columnIndex, numberIndex].Add((rowIndex, columnIndex));
                    }
                }
            }

            // --- Classic Box → Row/Column elimination ---
            for (int boxIndex = 0; boxIndex < 9; boxIndex++)
            {
                for (int n = 0; n < 9; n++)
                {
                    var positions = boxCandidates[boxIndex, n];

                    if (positions.Count < 2)
                    {
                        continue;
                    }

                    // All in same row?
                    if (positions.All(p => p.row == positions[0].row))
                    {
                        int rowIndex = positions[0].row;
                        var eliminations = new List<(int row, int col)>();
                        int startColumn = (boxIndex % 3) * 3;

                        for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                        {
                            if (columnIndex >= startColumn && columnIndex < startColumn + 3)
                            {
                                continue;
                            }

                            if (squares[rowIndex, columnIndex].PossibleNumbers.Contains(n + 1))
                            {
                                eliminations.Add((rowIndex, columnIndex));
                            }
                        }

                        if (eliminations.Count > 0)
                        {
                            int number = n + 1;
                            solveStep.Explanation = $"Pointing Pair: {number}s in box are all in row {rowIndex + 1}. Remove {number}s from other squares in row.";
                            solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                            solveStep.CandidatesRemovedNumbers.Add(n + 1);
                            positions.ForEach(p => solveStep.HighlightedSquares.Add(p));
                            eliminations.ForEach(e => solveStep.CandidatesRemovedSquares.Add(e));
                            return solveStep;
                        }
                    }

                    // All in same column?
                    if (positions.All(p => p.col == positions[0].col))
                    {
                        int columnIndex = positions[0].col;
                        var eliminations = new List<(int row, int col)>();
                        int startRow = (boxIndex / 3) * 3;

                        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                        {
                            if (rowIndex >= startRow && rowIndex < startRow + 3)
                            {
                                continue;
                            }

                            if (squares[rowIndex, columnIndex].PossibleNumbers.Contains(n + 1))
                            {
                                eliminations.Add((rowIndex, columnIndex));
                            }
                        }

                        if (eliminations.Count > 0)
                        {
                            int number = n + 1;
                            solveStep.Explanation = $"Pointing Pair: {number}s in box are all in column {columnIndex + 1}. Remove {number}s from other squares in column.";
                            solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                            solveStep.CandidatesRemovedNumbers.Add(n + 1);
                            positions.ForEach(p => solveStep.HighlightedSquares.Add(p));
                            eliminations.ForEach(e => solveStep.CandidatesRemovedSquares.Add(e));
                            return solveStep;
                        }
                    }
                }
            }

            // --- Extended Row → Box / Column → Box elimination ---
            for (int n = 0; n < 9; n++)
            {
                // Rows
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    var positions = rowCandidates[rowIndex, n];

                    if (positions.Count == 2)
                    {
                        int boxRow = positions[0].row / 3;
                        int boxCol = positions[0].col / 3;

                        if (positions.All(p => p.row / 3 == boxRow && p.col / 3 == boxCol))
                        {
                            var eliminations = new List<(int row, int col)>();
                            int startR = boxRow * 3;
                            int startC = boxCol * 3;

                            for (int rr = startR; rr < startR + 3; rr++)
                            {
                                for (int cc = startC; cc < startC + 3; cc++)
                                {
                                    if (positions.Contains((rr, cc)))
                                    {
                                        continue;
                                    }

                                    if (squares[rr, cc].PossibleNumbers.Contains(n + 1))
                                    {
                                        eliminations.Add((rr, cc));
                                    }
                                }
                            }

                            if (eliminations.Count > 0)
                            {
                                int number = n + 1;
                                solveStep.Explanation = $"Pointing Pair: {number}s in row {rowIndex + 1} are in same box. Remove {number}s from other squares in box.";
                                solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                                solveStep.CandidatesRemovedNumbers.Add(n + 1);
                                positions.ForEach(p => solveStep.HighlightedSquares.Add(p));
                                eliminations.ForEach(e => solveStep.CandidatesRemovedSquares.Add(e));
                                return solveStep;
                            }
                        }
                    }
                }

                // Columns
                for (int columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    var positions = colCandidates[columnIndex, n];

                    if (positions.Count == 2)
                    {
                        int boxRow = positions[0].row / 3;
                        int boxCol = positions[0].col / 3;

                        if (positions.All(p => p.row / 3 == boxRow && p.col / 3 == boxCol))
                        {
                            var eliminations = new List<(int row, int col)>();
                            int startR = boxRow * 3;
                            int startC = boxCol * 3;

                            for (int rr = startR; rr < startR + 3; rr++)
                            {
                                for (int cc = startC; cc < startC + 3; cc++)
                                {
                                    if (positions.Contains((rr, cc)))
                                    {
                                        continue;
                                    }

                                    if (squares[rr, cc].PossibleNumbers.Contains(n + 1))
                                    {
                                        eliminations.Add((rr, cc));
                                    }
                                }
                            }

                            if (eliminations.Count > 0)
                            {
                                int number = n + 1;
                                solveStep.Explanation = $"Pointing Pair: {number}s in column {columnIndex + 1} are in same box. Remove {number}s from other squares in box.";
                                solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                                solveStep.CandidatesRemovedNumbers.Add(n + 1);
                                positions.ForEach(p => solveStep.HighlightedSquares.Add(p));
                                eliminations.ForEach(e => solveStep.CandidatesRemovedSquares.Add(e));
                                return solveStep;
                            }
                        }
                    }
                }
            }

            return solveStep;
        }
    }
}
