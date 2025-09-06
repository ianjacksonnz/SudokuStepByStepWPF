using System.Windows.Controls;

namespace SudokuStepByStep.Models;

public class SudokuSquare
{
    public HashSet<int> PossibleNumbers { get; set; } = new HashSet<int>();
    public TextBox Box { get; set; }
    public TextBlock CandidatesBlock { get; set; }
    public Border Border { get; set; }

    public SudokuSquare(TextBox box, TextBlock candidatesBlock, Border border)
    {
        Box = box;
        CandidatesBlock = candidatesBlock;
        CandidatesBlock.Text = string.Join(" ", PossibleNumbers);
        Border = border;
    }
}