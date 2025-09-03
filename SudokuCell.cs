using System.Windows.Controls;

namespace SudokuWpfApp;

public class SudokuCell
{
    public TextBox Box { get; set; }
    public TextBlock CandidatesBlock { get; set; }
}
