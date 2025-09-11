using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SudokuStepByStep.ViewModels;

public class SudokuSquareViewModel : INotifyPropertyChanged
{
    private int _number;
    private bool _isReadOnly;
    private ObservableCollection<int> _possibleNumbers = new();

    public int Number
    {
        get => _number;
        set { _number = value; OnPropertyChanged(); }
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set { _isReadOnly = value; OnPropertyChanged(); }
    }

    public ObservableCollection<int> PossibleNumbers
    {
        get => _possibleNumbers;
        set { _possibleNumbers = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}