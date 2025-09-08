namespace SudokuStepByStep.Common;

public class Enums
{
    public enum SolvingRule
    {
        OnlyValueInSquare,
        OnlyInOnePlace,
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
