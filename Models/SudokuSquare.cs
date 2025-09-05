using System.Windows.Controls;

namespace SudokuStepByStep.Models;

public class SudokuSquare
{
    public required TextBox Box { get; set; }
    public required TextBlock CandidatesBlock { get; set; }
    public required Border Border { get; set; }

    public SudokuSquare(TextBox box, TextBlock candidatesBlock, Border border)
    {
        Box = box;
        CandidatesBlock = candidatesBlock;
        Border = border;
    }
}