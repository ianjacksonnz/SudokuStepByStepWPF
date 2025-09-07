using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class NakedTriples
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        var solveStep = new SolveStep()
        {
            Rule = Common.Enums.SolvingRule.NakedTriples
        };

        return solveStep;
    }
}
