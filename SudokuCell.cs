using System.Windows.Controls;

namespace SudokuStepByStep;

public class SudokuSquare
{
    public TextBox Box { get; init; }
    public TextBlock CandidatesBlock { get; init; }
    public Border Border { get; init; }

    public SudokuSquare(TextBox box, TextBlock candidates, Border border)
    {
        Box = box;
        CandidatesBlock = candidates;
        Border = border;
    }
}

