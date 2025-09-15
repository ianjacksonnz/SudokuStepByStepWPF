using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule
{
    public static class NakedTriples
    {
        public static SolveStep Run(SudokuSquareModel[,] squares)
        {
            return NakedGroup.Run(squares, 3, Enums.SolvingRule.NakedTriples);
        }
    }
}
