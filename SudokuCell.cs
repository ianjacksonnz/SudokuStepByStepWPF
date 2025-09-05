using System.Windows.Controls;

namespace SudokuHelper;

public class SudokuCell
{
    public required TextBox Box { get; set; }
    public required TextBlock CandidatesBlock { get; set; }
}
