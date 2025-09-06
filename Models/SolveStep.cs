namespace SudokuStepByStep.Models;

public class SolveStep
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int Value { get; set; }
    public bool IsSolved { get; set; }
    public Common.Enums.SolvingRule Method { get; set; }
    public int[,] HighlightedSquares { get; set; }

    public string Explanation { get; set; }
}
