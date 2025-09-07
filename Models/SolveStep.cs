namespace SudokuStepByStep.Models;

public class SolveStep
{
    public int Row { get; set; } = 0;
    public int Column { get; set; } = 0;
    public int Number { get; set; } = 0;
    public bool Solved { get; set; } = false;
    public bool CandidatesRemoved { get; set; } = false;
    public Common.Enums.SolvingRule Rule { get; set; }
    public HashSet<(int row, int col)> HighlightedSquares { get; set; } = new();

    public string Explanation { get; set; } = string.Empty;
}
