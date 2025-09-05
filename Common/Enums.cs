namespace SudokuHelper.Common;

public class Enums
{
    public enum SolvingMethod
    {
        OnlyValue,
        NakedPairs,
        PointingPairs
    }

    public enum CellGroupType
    {
        Column,
        Row,
        Grid
    }
}
