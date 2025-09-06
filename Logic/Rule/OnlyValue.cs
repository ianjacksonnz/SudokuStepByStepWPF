//using SudokuStepByStep.Common;
//using SudokuStepByStep.Logic.Helpers;
//using SudokuStepByStep.Models;

//namespace SudokuStepByStep.Logic.Rule;

//public static class OnlyValue
//{
//    public static SolveStep Run(SudokuSquare[,] squares)
//    {
//        numberSolved = 0;
//        rowSolved = -1;
//        columnSolved = -1;
//        groupType = null;

//        // --- rows ---
//        for (int row = 0; row < 9; row++)
//        {
//            for (int number = 1; number <= 9; number++)
//            {
//                var possibleColumns = new List<int>();

//                for (int column = 0; column < 9; column++)
//                {
//                    var square = squares[row, column];
//                    var isSafe = RulesHelper.IsSafe(squares, row, column, number);

//                    if (square.PossibleNumbers.Count > 0 &&
//                        square.PossibleNumbers.Contains(number) &&
//                        RulesHelper.IsSafe(squares, row, column, number))
//                        possibleColumns.Add(column);
//                }

//                if (possibleColumns.Count == 1)
//                {
//                    numberSolved = number;
//                    rowSolved = row;
//                    columnSolved = possibleColumns[0];
//                    groupType = Enums.SquareGroupType.Row;
//                    return true;
//                }
//            }
//        }

//        // --- columns ---
//        for (int column = 0; column < 9; column++)
//        {
//            for (int number = 1; number <= 9; number++)
//            {
//                var possibleRows = new List<int>();

//                for (int row = 0; row < 9; row++)
//                {
//                    if (grid[row, column].Count > 0 &&
//                        grid[row, column].Contains(number) &&
//                        RulesHelper.IsSafe(grid, row, column, number))
//                    {
//                        possibleRows.Add(row);
//                    }
//                }

//                if (possibleRows.Count == 1)
//                {
//                    numberSolved = number;
//                    rowSolved = possibleRows[0];
//                    columnSolved = column;
//                    groupType = Enums.SquareGroupType.Column;
//                    return true;
//                }
//            }
//        }

//        // --- grids ---
//        for (int boxRow = 0; boxRow < 3; boxRow++)
//        {
//            for (int boxCol = 0; boxCol < 3; boxCol++)
//            {
//                for (int number = 1; number <= 9; number++)
//                {
//                    var possibleSquares = new List<(int r, int c)>();

//                    for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
//                    {
//                        for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
//                        {
//                            if (grid[r, c].Count > 0 &&
//                                grid[r, c].Contains(number) &&
//                                RulesHelper.IsSafe(grid, r, c, number))
//                            {
//                                possibleSquares.Add((r, c));
//                            }
//                        }
//                    }

//                    if (possibleSquares.Count == 1)
//                    {
//                        numberSolved = number;
//                        rowSolved = possibleSquares[0].r;
//                        columnSolved = possibleSquares[0].c;
//                        groupType = Enums.SquareGroupType.Grid;
//                        return true;
//                    }
//                }
//            }
//        }

//        return false;
//    }
//    {
//        var solveStep = new SolveStep()
//        {
//            Row = 0,
//            Column = 0,
//            Value = 0,
//            IsSolved = false,
//            Method = Common.Enums.SolvingRule.OnlyValue,
//            HighlightedSquares = new int[9, 9],
//            Explanation = "This is a placeholder explanation."
//        };

//        return solveStep;
//    }
//}
