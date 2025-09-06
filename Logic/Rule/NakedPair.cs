using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class NakedPair
{
    public static SolveStep Run()
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.NakedPairs
        };

        return solveStep;
    }
}
