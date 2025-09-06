using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class NakedTriples
{
    public static SolveStep Run()
    {
        var solveStep = new SolveStep()
        {
            Rule = Common.Enums.SolvingRule.NakedTriples
        };

        return solveStep;
    }
}
