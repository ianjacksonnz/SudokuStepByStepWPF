using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic;

public static class RulesEngine
{
    public static SolveStep CalculateNextStep()
    {
        var solveStep = new SolveStep()
        {
            Row = 0,
            Column = 0,
            Value = 0,
            IsSolved = false,
            Method = Common.Enums.SolvingRule.NakedPairs,
            HighlightedSquares = new int[9, 9],
            Explanation = "This is a placeholder explanation."
        };

        return solveStep;
    }
}
