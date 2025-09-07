using SudokuStepByStep.Common;
using SudokuStepByStep.Logic.Rule;
using SudokuStepByStep.Models;

namespace SudokuStepByStep.Logic;

public static class RulesEngine
{
    public static SolveStep CalculateNextStep(SudokuSquare[,] squares)
    {
        foreach (Enums.SolvingRule rule in Enum.GetValues(typeof(Enums.SolvingRule)))
        {
            SolveStep step = rule switch
            {
                Enums.SolvingRule.OnlyValue => OnlyValue.Run(squares),
                Enums.SolvingRule.PointingPairs => PointingPairs.Run(squares),
                Enums.SolvingRule.PointingTriples => PointingTriples.Run(squares),
                Enums.SolvingRule.NakedPairs => NakedPair.Run(squares),
                Enums.SolvingRule.NakedTriples => NakedTriples.Run(squares),
                _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
            };

            if (step != null && (step.Solved || step.CandidatesRemoved))
            {
                return step;
            }
        }

        // If no step found, return a default step
        return null;
    }
}
