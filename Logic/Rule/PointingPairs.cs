using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule
{
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

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    foreach (var number in squares[r, c].PossibleNumbers)
                    {
                        int nIndex = number - 1;
                        int boxIndex = (r / 3) * 3 + (c / 3);

                        boxCandidates[boxIndex, nIndex].Add((r, c));
                        rowCandidates[r, nIndex].Add((r, c));
                        colCandidates[c, nIndex].Add((r, c));
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
                        int row = positions[0].row;
                        var eliminations = new List<(int row, int col)>();
                        int startColumn = (boxIndex % 3) * 3;

                        for (int c = 0; c < 9; c++)
                        {
                            if (c >= startColumn && c < startColumn + 3)
                            {
                                continue;
                            }

                            if (squares[row, c].PossibleNumbers.Contains(n + 1))
                            {
                                eliminations.Add((row, c));
                            }
                        }

                        if (eliminations.Count > 0)
                        {
                            solveStep.Explanation = $"Pointing Pair: {n + 1}s in box are all in row {row + 1}, remove from row.";
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
                        int col = positions[0].col;
                        var eliminations = new List<(int row, int col)>();
                        int startRow = (boxIndex / 3) * 3;

                        for (int r = 0; r < 9; r++)
                        {
                            if (r >= startRow && r < startRow + 3)
                            {
                                continue;
                            }

                            if (squares[r, col].PossibleNumbers.Contains(n + 1))
                            {
                                eliminations.Add((r, col));
                            }
                        }

                        if (eliminations.Count > 0)
                        {
                            solveStep.Explanation = $"Pointing Pair: {n + 1}s in box are all in column {col + 1}, remove from column.";
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
                for (int r = 0; r < 9; r++)
                {
                    var positions = rowCandidates[r, n];

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
                                solveStep.Explanation = $"Pointing Pair: {n + 1}s in row {r + 1} are in same box, remove from other squares in box.";
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
                for (int c = 0; c < 9; c++)
                {
                    var positions = colCandidates[c, n];

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
                                solveStep.Explanation = $"Pointing Pair: {n + 1}s in column {c + 1} are in same box, remove from other squares in box.";
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
