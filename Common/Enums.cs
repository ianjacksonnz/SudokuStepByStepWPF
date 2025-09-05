namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingMethod
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
