using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule
{
    public static class NakedQuads
    {
        public static SolveStep Run(SudokuSquare[,] squares)
        {
            return NakedGroup.Run(squares, 4, Enums.SolvingRule.NakedQuads);
        }
    }
}
