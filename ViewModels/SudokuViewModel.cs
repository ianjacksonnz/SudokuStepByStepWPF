using SudokuStepByStep.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SudokuStepByStep.ViewModels;

public class SudokuViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ObservableCollection<SudokuSquareModel>> Grid { get; set; } = new();
    public ObservableCollection<string> PuzzleNames { get; set; } = new();
    private string _selectedPuzzle;

    public string SelectedPuzzle
    {
        get => _selectedPuzzle;
        set
        {
            _selectedPuzzle = value;
            OnPropertyChanged();
            LoadPuzzleCommand.Execute(null);
        }
    }

    public ICommand StepCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand NewPuzzleCommand { get; }
    public ICommand LoadPuzzleCommand { get; }

    public SudokuViewModel()
    {
        InitializeGrid();

        StepCommand = new RelayCommand(Step);
        ClearCommand = new RelayCommand(Clear);
        NewPuzzleCommand = new RelayCommand(NewPuzzle);
        LoadPuzzleCommand = new RelayCommand(LoadSelectedPuzzle);

        LoadPuzzles();
    }

    private void InitializeGrid()
    {
        Grid.Clear();
        for (int row = 0; row < 9; row++)
        {
            var rowList = new ObservableCollection<SudokuSquareModel>();
            for (int col = 0; col < 9; col++)
            {
                rowList.Add(new SudokuSquareModel { Row = row, Column = col });
            }
            Grid.Add(rowList);
        }
    }

    private void LoadPuzzles()
    {
        // Example: replace with your PuzzleLoader logic
        var puzzles = SudokuStepByStep.Logic.Helpers.PuzzleLoader.GetPuzzles();

        PuzzleNames.Clear();
        foreach (var key in puzzles.Keys)
            PuzzleNames.Add(key);

        SelectedPuzzle = PuzzleNames.Count > 0 ? PuzzleNames[0] : null!;
    }

    private void LoadSelectedPuzzle()
    {
        if (string.IsNullOrEmpty(SelectedPuzzle)) return;

        var puzzles = SudokuStepByStep.Logic.Helpers.PuzzleLoader.GetPuzzles();
        if (!puzzles.TryGetValue(SelectedPuzzle, out var puzzle)) return;

        for (int row = 0; row < 9; row++)
            for (int col = 0; col < 9; col++)
            {
                var square = Grid[row][col];
                square.Number = puzzle[row, col];
                square.IsReadOnly = puzzle[row, col] != 0;
                square.BackgroundColor = System.Windows.Media.Brushes.White;
            }

        // Initialize candidates
        SudokuStepByStep.Logic.Helpers.RulesHelper.SetPossibleNumbers(GridToSquaresArray());
    }

    private void Step()
    {
        // Example: call your RulesEngine logic using Grid
        var squaresArray = GridToSquaresArray();
        var solveStep = SudokuStepByStep.Logic.RulesEngine.CalculateNextStep(squaresArray);

        if (solveStep != null)
        {
            // Update Grid with step result
            UpdateGridFromSolveStep(solveStep);
        }
    }

    private void Clear()
    {
        foreach (var row in Grid)
            foreach (var square in row)
            {
                square.Number = 0;
                square.IsReadOnly = false;
                square.BackgroundColor = System.Windows.Media.Brushes.White;
                square.PossibleNumbers.Clear();
            }
    }

    private void NewPuzzle()
    {
        Clear();
        SelectedPuzzle = null!;
    }

    private SudokuStepByStep.Models.SudokuSquareModel[,] GridToSquaresArray()
    {
        var arr = new SudokuSquareModel[9, 9];

        for (int row = 0; row < 9; row++)
            for (int col = 0; col < 9; col++)
            {
                var m = Grid[row][col];
                arr[row, col] = new SudokuSquareModel
                {
                    Number = m.Number,
                    PossibleNumbers = new System.Collections.ObjectModel.ObservableCollection<int>(m.PossibleNumbers)
                };
            }

        return arr;
    }

    private void UpdateGridFromSolveStep(SolveStep solveStep)
    {
        var square = Grid[solveStep.Row][solveStep.Column];
        if (solveStep.Solved)
        {
            square.Number = solveStep.Number;
            square.PossibleNumbers.Clear();
            square.BackgroundColor = System.Windows.Media.Brushes.LightGreen;
        }

        // Highlight other squares
        foreach (var (r, c) in solveStep.HighlightedSquares)
        {
            Grid[r][c].BackgroundColor = System.Windows.Media.Brushes.LightYellow;
        }

        foreach (var (r, c) in solveStep.CandidatesRemovedSquares)
        {
            Grid[r][c].BackgroundColor = System.Windows.Media.Brushes.LightBlue;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
