using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SudokuStepByStep.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public ObservableCollection<string> PuzzleNames { get; } = new();
    public ObservableCollection<ObservableCollection<SudokuSquareViewModel>> Squares { get; } = new();
    public ICommand StepCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand NewPuzzleCommand { get; }
    private string? _selectedPuzzle;
    public string? SelectedPuzzle
    {
        get => _selectedPuzzle;
        set
        {
            if (_selectedPuzzle != value)
            {
                _selectedPuzzle = value;
                OnPropertyChanged();
                if (value != null)
                    LoadPuzzle(value);
            }
        }
    }

    public MainWindowViewModel()
    {
        StepCommand = new RelayCommand(_ => Step());
        ClearCommand = new RelayCommand(_ => Clear());
        NewPuzzleCommand = new RelayCommand(_ => NewPuzzle());

        // Initialize grid
        for (int r = 0; r < 9; r++)
        {
            var row = new ObservableCollection<SudokuSquareViewModel>();
            for (int c = 0; c < 9; c++)
                row.Add(new SudokuSquareViewModel());
            Squares.Add(row);
        }

        // Load puzzles
        var puzzles = Logic.Helpers.PuzzleLoader.GetPuzzles();
        foreach (var name in puzzles.Keys)
            PuzzleNames.Add(name);

        // Optionally select a default puzzle
        SelectedPuzzle = PuzzleNames.Count > 0 ? PuzzleNames[0] : null;
    }

    private void LoadPuzzle(string puzzleName)
    {
        var puzzles = Logic.Helpers.PuzzleLoader.GetPuzzles();
        if (!puzzles.TryGetValue(puzzleName, out var puzzle)) return;

        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
            {
                var square = Squares[r][c];
                square.Number = puzzle[r, c];
                square.IsReadOnly = puzzle[r, c] != 0;
                square.PossibleNumbers.Clear();
            }
    }

    private void Step()
    {
        // Implement step logic using your RulesEngine and update Squares accordingly
    }

    private void Clear()
    {
        foreach (var row in Squares)
            foreach (var square in row)
            {
                square.Number = 0;
                square.IsReadOnly = false;
                square.PossibleNumbers.Clear();
            }
    }

    private void NewPuzzle()
    {
        Clear();
        SelectedPuzzle = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

// Add the following RelayCommand implementation if it is missing in your project
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
