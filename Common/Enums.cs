namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingRule
    {
        NakedSingle,
        HiddenSingle,
        NakedPairs,
        PointingPairs,
        HiddenPairs,
        NakedTriples,
        NakedQuads,
        XWing
    }

    public enum SquareGroupType
    {
        Column,
        Row,
        Box
    }
}
