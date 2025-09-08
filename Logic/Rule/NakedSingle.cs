using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

/// <summary>
/// Only one possible number can appear in the square
/// </summary>
public static class NakedSingle
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.NakedSingle,
        };

        var gridPossibleNumbers = GridHelper.GetPossibleNumbers(squares);

        // --- rows ---
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                var square = squares[row, column];

                if (square.PossibleNumbers.Count == 1)
                {
                    var number = square.PossibleNumbers.First();

                    solveStep.Solved = true;
                    solveStep.Number = number;
                    solveStep.Row = row;
                    solveStep.Column = column;
                    solveStep.Explanation = $"The number {number} is the only number that can go in this square.";

                    return solveStep;
                }       
            }
        }

        return solveStep;
    }
}
