namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingRule
    {
        NakedSingle,
        HiddenSingle,
        PointingPairs,
        PointingTriples,
        NakedPairs,
        NakedTriples
    }

    public enum SquareGroupType
    {
        Column,
        Row,
        Grid
    }
}
