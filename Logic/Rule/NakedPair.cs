using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule;

public static class NakedPair
{
    public static SolveStep Run(SudokuSquare[,] squares)
    {
        var solveStep = new SolveStep()
        {
            Rule = Enums.SolvingRule.NakedPairs
        };

        return solveStep;
    }
}
