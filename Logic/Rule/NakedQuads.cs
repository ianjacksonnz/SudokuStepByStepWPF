using SudokuStepByStep.Common;
using SudokuStepByStep.Models;
using System.Collections.Generic;
using System.Linq;

namespace SudokuStepByStep.Logic.Rule
{
    public static class NakedQuads
    {
        public static SolveStep Run(SudokuSquare[,] squares)
        {
            var solveStep = new SolveStep()
            {
                Rule = Enums.SolvingRule.NakedQuads,
                HighlightedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedSquares = new HashSet<(int row, int col)>(),
                CandidatesRemovedNumbers = new List<int>()
            };

            // Helper to populate HighlightedSquares based on group type
            void PopulateHighlightedSquares(Enums.SquareGroupType groupType, HashSet<(int row, int col)> candidatesRemovedSquares, int groupIndexRow = 0, int groupIndexCol = 0)
            {
                switch (groupType)
                {
                    case Enums.SquareGroupType.Row:
                        for (int col = 0; col < 9; col++)
                        {
                            if (!candidatesRemovedSquares.Contains((groupIndexRow, col)))
                            {
                                solveStep.HighlightedSquares.Add((groupIndexRow, col));
                            }
                        }
                        break;
                    case Enums.SquareGroupType.Column:
                        for (int row = 0; row < 9; row++)
                        {
                            if (!candidatesRemovedSquares.Contains((row, groupIndexCol)))
                            {
                                solveStep.HighlightedSquares.Add((row, groupIndexCol));
                            }
                        }
                        break;
                    case Enums.SquareGroupType.Box:
                        int startRow = groupIndexRow * 3;
                        int startCol = groupIndexCol * 3;
                        for (int r = startRow; r < startRow + 3; r++)
                        {
                            for (int c = startCol; c < startCol + 3; c++)
                            {
                                if (!candidatesRemovedSquares.Contains((r, c)))
                                {
                                    solveStep.HighlightedSquares.Add((r, c));
                                }
                            }
                        }
                        break;
                }
            }

            // Helper to process a group of 4 cells
            void CheckGroup(List<(int row, int col)> cells, List<(int row, int col)> allCells, Enums.SquareGroupType groupType, int groupIndexRow = 0, int groupIndexCol = 0)
            {
                var quadCandidates = new HashSet<int>();

                foreach (var cell in cells)
                {
                    quadCandidates.UnionWith(squares[cell.row, cell.col].PossibleNumbers);
                }

                if (quadCandidates.Count == 4)
                {
                    if (cells.All(cell => squares[cell.row, cell.col].PossibleNumbers.All(n => quadCandidates.Contains(n))))
                    {
                        foreach (var cell in allCells.Except(cells))
                        {
                            foreach (var n in quadCandidates)
                            {
                                if (squares[cell.row, cell.col].PossibleNumbers.Contains(n))
                                {
                                    solveStep.CandidatesRemovedSquares.Add(cell);

                                    if (!solveStep.CandidatesRemovedNumbers.Contains(n))
                                    {
                                        solveStep.CandidatesRemovedNumbers.Add(n);
                                        solveStep.CandidatesRemovedInNonHighlightedSquares = true;
                                    }
                                }
                            }
                        }
                        if (solveStep.CandidatesRemovedSquares.Count > 0)
                        {
                            PopulateHighlightedSquares(groupType, solveStep.CandidatesRemovedSquares, groupIndexRow, groupIndexCol);
                            solveStep.Explanation = $"Naked Quad: Candidates {string.Join(", ", quadCandidates)} only appear in four cells in the same {groupType.ToString().ToLower()}. Remove these candidates from other cells in the {groupType.ToString().ToLower()}.";
                        }
                    }
                }
            }

            // Check rows
            for (int row = 0; row < 9; row++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(col => squares[row, col].PossibleNumbers.Count >= 2 && squares[row, col].PossibleNumbers.Count <= 4)
                    .Select(col => (row, col)).ToList();

                if (cells.Count >= 4)
                {
                    foreach (var quad in cells.Combinations(4))
                    {
                        CheckGroup(quad, Enumerable.Range(0, 9).Select(col => (row, col)).ToList(), Enums.SquareGroupType.Row, row);

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                        {
                            return solveStep;
                        }
                    }
                }
            }

            // Check columns
            for (int col = 0; col < 9; col++)
            {
                var cells = Enumerable.Range(0, 9)
                    .Where(row => squares[row, col].PossibleNumbers.Count >= 2 && squares[row, col].PossibleNumbers.Count <= 4)
                    .Select(row => (row, col)).ToList();

                if (cells.Count >= 4)
                {
                    foreach (var quad in cells.Combinations(4))
                    {
                        CheckGroup(quad, Enumerable.Range(0, 9).Select(row => (row, col)).ToList(), Enums.SquareGroupType.Column, 0, col);

                        if (!string.IsNullOrEmpty(solveStep.Explanation))
                        {
                            return solveStep;
                        }
                    }
                }
            }

            // Check boxes
            for (int boxRow = 0; boxRow < 3; boxRow++)
            {
                for (int boxCol = 0; boxCol < 3; boxCol++)
                {
                    var boxCells = new List<(int row, int col)>();

                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                    {
                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                        {
                            if (squares[r, c].PossibleNumbers.Count >= 2 && squares[r, c].PossibleNumbers.Count <= 4)
                            {
                                boxCells.Add((r, c));
                            }
                        }
                    }

                    if (boxCells.Count >= 4)
                    {
                        foreach (var quad in boxCells.Combinations(4))
                        {
                            CheckGroup(quad, Enumerable.Range(boxRow * 3, 3)
                                .SelectMany(r => Enumerable.Range(boxCol * 3, 3).Select(c => (r, c))).ToList(),
                                Enums.SquareGroupType.Box, boxRow, boxCol);

                            if (!string.IsNullOrEmpty(solveStep.Explanation))
                            {
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
