namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingMethod
    {
        OnlyValue,
        NakedPairs,
        PointingPairs
    }

    public enum SquareGroupType
    {
        Column,
        Row,
        Grid
    }
}
