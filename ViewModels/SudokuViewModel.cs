using SudokuStepByStep.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Configuration;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Logic;

namespace SudokuStepByStep.ViewModels;

public class SudokuViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ObservableCollection<SudokuSquare>> Grid { get; set; } = new();
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

    private bool _showPossibleNumbers;
    public bool ShowPossibleNumbers
    {
        get => _showPossibleNumbers;
        set
        {
            if (_showPossibleNumbers != value)
            {
                _showPossibleNumbers = value;
                OnPropertyChanged();
            }
        }
    }

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

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            var rowList = new ObservableCollection<SudokuSquare>();

            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                rowList.Add(new SudokuSquare { Row = rowIndex, Column = columnIndex });
            }

            Grid.Add(rowList);
        }
    }

    private void LoadPuzzles()
    {
        var puzzles = PuzzleLoader.GetPuzzles();

        PuzzleNames.Clear();

        foreach (var key in puzzles.Keys)
        {
            PuzzleNames.Add(key);
        }

        var defaultPuzzleName = ConfigurationManager.AppSettings["DefaultPuzzleName"];

        if (!string.IsNullOrEmpty(defaultPuzzleName) && PuzzleNames.Contains(defaultPuzzleName))
        {
            SelectedPuzzle = defaultPuzzleName;
        }
        else
        {
            SelectedPuzzle = PuzzleNames.Count > 0 ? PuzzleNames[0] : null!;
        }

        ShowPossibleNumbers = false; // Hide possible numbers after loading puzzle
    }

    private void LoadSelectedPuzzle()
    {
        if (string.IsNullOrEmpty(SelectedPuzzle)) return;

        var puzzles = PuzzleLoader.GetPuzzles();

        if (!puzzles.TryGetValue(SelectedPuzzle, out var puzzle))
        {
            return;
        }

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var square = Grid[rowIndex][columnIndex];
                square.Number = puzzle[rowIndex, columnIndex];
                square.IsReadOnly = puzzle[rowIndex, columnIndex] != 0;
                square.BackgroundColor = System.Windows.Media.Brushes.White;
            }
        }

        // Initialize candidates
        var sodukoSquares = GridToSquaresArray();
        RulesHelper.SetPossibleNumbers(sodukoSquares);
    }

    private void Step()
    {
        // Example: call your RulesEngine logic using Grid
        var squaresArray = GridToSquaresArray();
        var solveStep = RulesEngine.CalculateNextStep(squaresArray);

        if (solveStep != null)
        {
            // Update Grid with step result
            UpdateGridFromSolveStep(solveStep);
        }

        ShowPossibleNumbers = true; // Show possible numbers after stepping
    }

    private void Clear()
    {
        var sodukoSquares = GridToSquaresArray();
        GridHelper.ClearSquares(sodukoSquares);
        RulesHelper.SetPossibleNumbers(sodukoSquares);
    }

    private void NewPuzzle()
    {
        Clear();
        SelectedPuzzle = null!;
    }

    private SudokuSquare[,] GridToSquaresArray()
    {
        var sodukoSquares = new SudokuSquare[9, 9];

        for (int rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                var square = Grid[rowIndex][columnIndex];
                var possibleNumbers = new ObservableCollection<int>(square.PossibleNumbers);

                sodukoSquares[rowIndex, columnIndex] = Grid[rowIndex][columnIndex];
            }
        }

        return sodukoSquares;
    }

    private void UpdateGridFromSolveStep(SolveStep solveStep)
    {
        var square = Grid[solveStep.Row][solveStep.Column];

        if (solveStep.Solved)
        {
            square.Number = solveStep.Number;
            square.PossibleNumbers.Clear();
            square.BackgroundColor = System.Windows.Media.Brushes.LightGreen;

            var sodukoSquares = GridToSquaresArray();
            RulesHelper.RemovePossibleNumbersFromGridAfterSquareSolved(sodukoSquares, solveStep);
        }

        // Highlight other squares
        foreach (var (rowIndex, columnIndex) in solveStep.HighlightedSquares)
        {
            Grid[rowIndex][columnIndex].BackgroundColor = System.Windows.Media.Brushes.LightYellow;
        }

        foreach (var (rowIndex, columnIndex) in solveStep.CandidatesRemovedSquares)
        {
            Grid[rowIndex][columnIndex].BackgroundColor = System.Windows.Media.Brushes.LightBlue;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
