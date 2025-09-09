namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingRule
    {
        NakedSingle,
        HiddenSingle,
        PointingPairs,
        HiddenPairs,
        XWing
    }

    public enum SquareGroupType
    {
        Column,
        Row,
        Grid
    }
}
