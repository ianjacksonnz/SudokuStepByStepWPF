namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingRule
    {
        NakedSingle,
        HiddenSingle,
        PointingPairs,
        HiddenPairs,
        NakedQuads,
        XWing
    }

    public enum SquareGroupType
    {
        Column,
        Row,
        Grid
    }
}
