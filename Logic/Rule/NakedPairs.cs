using SudokuStepByStep.Common;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic.Rule
{
    public static class NakedPairs
    {
        public static SolveStep Run(SudokuSquare[,] squares)
        {
            return NakedGroup.Run(squares, 2, Enums.SolvingRule.NakedPairs);
        }
    }
}
