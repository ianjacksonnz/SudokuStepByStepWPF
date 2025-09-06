using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class PointingPairs
{
    public static SolveStep Run()
    {
        var solveStep = new SolveStep()
        {
            Rule = Common.Enums.SolvingRule.PointingPairs
        };

        return solveStep;
    }
}
