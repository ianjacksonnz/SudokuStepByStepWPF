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

            // loop through each 3x3 box
            for (int boxRow = 0; boxRow < 3; boxRow++)
            {
                for (int boxColumn = 0; boxColumn < 3; boxColumn++)
                {
                    int startRow = boxRow * 3;
                    int startColumn = boxColumn * 3;

                    for (int number = 1; number <= 9; number++)
                    {
                        var positions = new List<(int row, int col)>();

                        // collect all positions inside the box that can contain the number
                        for (int r = startRow; r < startRow + 3; r++)
                        {
                            for (int c = startColumn; c < startColumn + 3; c++)
                            {
                                var square = squares[r, c];

                                if (square.PossibleNumbers.Contains(number))
                                {
                                    positions.Add((r, c));
                                }
                            }
                        }

                        if (positions.Count < 2) continue; // need at least 2 to form a pointing pair

                        // check if all positions are in the same row
                        bool sameRow = positions.All(p => p.row == positions[0].row);

                        if (sameRow)
                        {
                            int row = positions[0].row;

                            // eliminate this number from the rest of the row outside the box
                            var eliminations = new List<(int row, int col)>();

                            for (int c = 0; c < 9; c++)
                            {
                                if (c >= startColumn && c < startColumn + 3)
                                {
                                    continue; // skip the box
                                }

                                if (squares[row, c].PossibleNumbers.Contains(number))
                                {
                                    eliminations.Add((row, c));
                                }
                            }

                            if (eliminations.Count > 0)
                            {
                                solveStep.Explanation = $"Pointing Pair: {number}s in the box are all in row {row + 1}, so remove {number} from that row.";
                                solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                                solveStep.CandidatesRemovedNumbers.Add(number);

                                foreach (var position in positions)
                                {
                                    solveStep.HighlightedSquares.Add(position);
                                }

                                foreach (var candidateRemovedSquare in eliminations)
                                {
                                    solveStep.CandidatesRemovedSquares.Add(candidateRemovedSquare);
                                }
                               
                                return solveStep;
                            }
                        }

                        // check if all positions are in the same column
                        bool sameColumn = positions.All(p => p.col == positions[0].col);

                        if (sameColumn)
                        {
                            int column = positions[0].col;

                            // eliminate this number from the rest of the column outside the box
                            var eliminations = new List<(int row, int col)>();

                            for (int r = 0; r < 9; r++)
                            {
                                if (r >= startRow && r < startRow + 3)
                                {
                                    continue; // skip the box
                                }

                                if (squares[r, column].PossibleNumbers.Contains(number))
                                {
                                    eliminations.Add((r, column));
                                }
                            }

                            if (eliminations.Count > 0)
                            {
                                solveStep.Explanation = $"Pointing Pair: {number}s in the box are all in column {column + 1}, so remove {number} from that column.";
                                solveStep.CandidatesRemovedInNonHighlightedSquares = true;               
                                solveStep.CandidatesRemovedNumbers.Add(number);                               

                                foreach (var position in positions)
                                {
                                    solveStep.HighlightedSquares.Add(position);
                                }

                                foreach (var candidateRemovedSquare in eliminations)
                                {
                                    solveStep.CandidatesRemovedSquares.Add(candidateRemovedSquare);
                                }
                            
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
