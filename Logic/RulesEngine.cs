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
                Enums.SolvingRule.NakedSingle => NakedSingle.Run(squares),
                Enums.SolvingRule.NakedPairs => NakedPairs.Run(squares),
                Enums.SolvingRule.HiddenSingle => HiddenSingle.Run(squares),            
                Enums.SolvingRule.PointingPairs => PointingPairs.Run(squares),
                Enums.SolvingRule.HiddenPairs => HiddenPairs.Run(squares),
                Enums.SolvingRule.NakedTriples => NakedTriples.Run(squares),
                Enums.SolvingRule.HiddenTriples => HiddenTriples.Run(squares),
                Enums.SolvingRule.NakedQuads => NakedQuads.Run(squares),
                Enums.SolvingRule.XWing => XWing.Run(squares),
                _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
            };

            if (step != null && (step.Solved || step.CandidatesRemovedInNonHighlightedSquares || step.CandidatesRemovedInHighlightedSquares))
            {
                return step;
            }
        }

        // If no step found, return a default step
        return null;
    }
}
