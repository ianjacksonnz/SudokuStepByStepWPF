namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingRule
    {
        OnlyValue,
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
