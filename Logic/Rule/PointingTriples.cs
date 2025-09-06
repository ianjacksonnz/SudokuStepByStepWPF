using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class PointingTriples
{
    public static SolveStep Run()
    {
        var solveStep = new SolveStep()
        {
            Rule = Common.Enums.SolvingRule.PointingTriples
        };

        return solveStep;
    }
}
