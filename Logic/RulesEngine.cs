using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic;

public static class RulesEngine
{
    public static SolveStep CalculateNextStep()
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.OnlyValue
        };

        return solveStep;
    }
}
