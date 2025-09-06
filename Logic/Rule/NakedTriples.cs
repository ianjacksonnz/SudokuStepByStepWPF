using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class NakedTriples
{
    public static SolveStep Run()
    {
        var solveStep = new SolveStep()
        {
            Row = 0,
            Column = 0,
            Value = 0,
            IsSolved = false,
            Method = Common.Enums.SolvingRule.NakedTriples,
            HighlightedSquares = new int[9, 9],
            Explanation = "This is a placeholder explanation."
        };

        return solveStep;
    }
}
