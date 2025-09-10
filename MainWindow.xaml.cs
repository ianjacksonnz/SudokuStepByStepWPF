using SudokuStepByStep.Common;
using SudokuStepByStep.Logic;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuStepByStep;

public partial class MainWindow : Window
{
    private Dictionary<string, int[,]> _puzzles = new Dictionary<string, int[,]>();
    private SudokuSquare[,] _squares = new SudokuSquare[9, 9];
    private Popup? _stepPopup = null!;
    private TextBox? _previousStepBox = null!;
    private List<int> _previousStepCandidatesRemovedNumbers = [];
    private bool _previousStepCandidatesRemoved = false;
    private HashSet<(int row, int col)> _previousStepCandidatesRemovedSquares = new();
    private bool _isNew = false;

    public MainWindow()
    {
        InitializeComponent();

        InitializeGrid();
        CreatePuzzles();
        
        // Close hint tooltip on outside clicks
        this.PreviewMouseDown += MainWindow_PreviewMouseDown;
    }

    private void CreatePuzzles()
    {
        _puzzles = PuzzleLoader.GetPuzzles();

        foreach (var key in _puzzles.Keys)
        {
            PuzzleSelector.Items.Add(key);
        }

        PuzzleSelector.SelectedIndex = 4; // Genius Puzzle 1
    }

    private void InitializeGrid()
    {
        SudokuGrid.Children.Clear();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var border = new Border
                {
                    BorderThickness = new Thickness(
                        col % 3 == 0 ? 2 : 0.5,
                        row % 3 == 0 ? 2 : 0.5,
                        (col + 1) % 3 == 0 ? 2 : 0.5,
                        (row + 1) % 3 == 0 ? 2 : 0.5),
                    BorderBrush = Brushes.Black
                };

                var grid = new Grid();

                // Candidates TextBlock (hidden by default)
                var candidates = new TextBlock
                {
                    FontSize = 10,
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(2),
                    IsHitTestVisible = false,
                    Visibility = Visibility.Collapsed, // hide initially
                    Background = Brushes.Transparent,
                    Text = string.Empty
                };

                var tb = new TextBox
                {
                    FontSize = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Brushes.White
                };

                grid.Children.Add(tb);
                grid.Children.Add(candidates);

                border.Child = grid;

                int r = row, c = col;
                // tb.TextChanged += (s, e) => RulesHelper.SetPossibleValues(_squares, false);
                tb.LostFocus += (s, e) => ClearHighlighting();

                _squares[row, col] = new SudokuSquare(tb, candidates, border);

                SudokuGrid.Children.Add(border);
            }
        }
    }

    private void PuzzleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ResetPreviousStepStoredValues();

        if (PuzzleSelector.SelectedItem == null) { return; }

        string? selected = PuzzleSelector.SelectedItem.ToString();

        if (selected != null && _puzzles.TryGetValue(selected, out int[,]? value))
        {
            LoadPuzzle(value);
        }
    }

    private void LoadPuzzle(int[,] puzzle)
    {
        _isNew = false;
        PuzzleLoader.LoadPuzzle(puzzle, _squares);
        ClearHighlighting();
        RulesHelper.SetPossibleNumbers(_squares, false);
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ResetPreviousStepStoredValues();
        GridHelper.ClearSquares(_squares);
        RulesHelper.SetPossibleNumbers(_squares, false);
    }

    private void ResetPreviousStepStoredValues()
    {
        _previousStepBox = null!;
        _previousStepCandidatesRemovedNumbers = [];
        _previousStepCandidatesRemoved = false;
        _previousStepCandidatesRemovedSquares = new();
    }


    private void ClearHighlighting()
    {
        foreach (var square in _squares)
        {
            square.Box.Background = Brushes.White;
        }

        // _highlightedSquares.Clear();

        if (_stepPopup != null)
        {
            _stepPopup.IsOpen = false;
        }
    }

    private void Step_Click(object sender, RoutedEventArgs e)
    {
        ClearHighlighting();

        if (RulesHelper.PuzzleSolved(_squares))
        {
            MessageBox.Show("Puzzle Solved!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
            RulesHelper.SetPossibleNumbers(_squares, false);

            // Remove focus from the solved cell
            Keyboard.ClearFocus();
            return;
        }

        if (_previousStepCandidatesRemoved)
        {
            foreach (var (r, c) in _previousStepCandidatesRemovedSquares)
            {
                var squareRemoveCandidates = _squares[r, c];

                foreach (var number in _previousStepCandidatesRemovedNumbers)
                {
                    RulesHelper.RemovePossibleNumbersFromSquare(_squares, squareRemoveCandidates, number);
                }          
            }
        }

        var solveStep = RulesEngine.CalculateNextStep(_squares);

        if ((solveStep != null) && (solveStep.Solved || solveStep.CandidatesRemovedInHighlightedSquares || solveStep.CandidatesRemovedInNonHighlightedSquares))
        {
            UpdateGridSolvedStep(solveStep);        
        }
        else
        {
            MessageBox.Show("The puzzle can not be solved!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        RulesHelper.ShowPossibleValues(_squares, true);
    }

    private void UpdateGridSolvedStep(SolveStep solveStep)
    {
        var square = _squares[solveStep.Row, solveStep.Column];
        _previousStepCandidatesRemovedNumbers = solveStep.CandidatesRemovedNumbers;
        _previousStepCandidatesRemoved = solveStep.CandidatesRemovedInHighlightedSquares || solveStep.CandidatesRemovedInNonHighlightedSquares;

        // Replace this line wherever you need the described logic
        _previousStepCandidatesRemovedSquares =
            solveStep.CandidatesRemovedInHighlightedSquares
                ? new HashSet<(int row, int col)>(solveStep.HighlightedSquares)
                : solveStep.CandidatesRemovedInNonHighlightedSquares
                    ? new HashSet<(int row, int col)>(solveStep.CandidatesRemovedSquares)
                    : new HashSet<(int row, int col)>();

        // _previousStepCandidatesRemovedSquares = new HashSet<(int row, int col)>(solveStep.CandidatesRemovedSquares);

        if (solveStep.Solved)
        {
            square.Number = solveStep.Number;
            square.PossibleNumbers.Clear();

            var box = _squares[solveStep.Row, solveStep.Column].Box;
            box.Text = solveStep.Number.ToString();

            RulesHelper.RemovePossibleNumbersFromGridAfterSolvedSquare(_squares, solveStep);

            if (_previousStepCandidatesRemoved)
            {
                foreach (var (r, c) in _previousStepCandidatesRemovedSquares)
                {
                    var squareRemoveCandidates = _squares[r, c];
                    RulesHelper.RemovePossibleNumbersFromSquare(_squares, squareRemoveCandidates, solveStep.Number);
                }
            }
        }

        var squareHighlightedBox = SetToolTip(solveStep);

        // Remove focus from the solved cell and do not attach key handlers
        _previousStepBox = squareHighlightedBox;
        //_prevStepBox.KeyDown += StepSquare_KeyDown;
        //_prevStepBox.TextChanged += StepSquare_TextChanged;
        // Do not set focus
        Keyboard.ClearFocus();
    }

    private void SetHighlighting(SolveStep solveStep)
    {
        if (solveStep.Solved)
        {
            var box = _squares[solveStep.Row, solveStep.Column].Box;
            box.Background = Brushes.LightGreen;
        }

        // Highlighted squares
        foreach (var (r, c) in solveStep.HighlightedSquares)
        {
            var box = _squares[r, c].Box;
            box.Background = Brushes.LightYellow;
        }

        // Candidates removed squares
        foreach (var (r, c) in solveStep.CandidatesRemovedSquares)
        {
            var box = _squares[r, c].Box;
            box.Background = Brushes.LightBlue;

        }
    }

    private TextBox SetToolTip(SolveStep solveStep)
    {
        TextBox squareHighlightedBox;
        int toolTipColumn = 0;

        if (solveStep.Solved)
        {
            squareHighlightedBox = _squares[solveStep.Row, solveStep.Column].Box;
            toolTipColumn = solveStep.Column;
        }
        else
        {
            var firstHighlightedSquare = solveStep.HighlightedSquares.FirstOrDefault();
            squareHighlightedBox = _squares[firstHighlightedSquare.row, firstHighlightedSquare.col].Box;
            toolTipColumn = firstHighlightedSquare.col;
        }

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var border = new Border
        {
            Background = Brushes.LightYellow,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(6),
            MinWidth = squareHighlightedBox.ActualWidth > 0 ? squareHighlightedBox.ActualWidth : 40,
            Child = new TextBlock
            {
                Text = solveStep.Explanation,
                Foreground = Brushes.Black,
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            }
        };

        stack.Children.Add(border);

        var arrow = new System.Windows.Shapes.Polygon
        {
            Points = new PointCollection { new Point(0, 0), new Point(12, 0), new Point(6, 8) },
            Fill = Brushes.LightYellow,
            Stroke = Brushes.Gray,
            StrokeThickness = 1,
            Margin = new Thickness(0, -1, 0, 0),
            Width = 12,
            Height = 8
        };

        stack.Children.Add(arrow);

        var popup = new Popup
        {
            Child = stack,
            PlacementTarget = squareHighlightedBox,
            Placement = PlacementMode.Top,
            StaysOpen = true,
            AllowsTransparency = true,
            PopupAnimation = PopupAnimation.Fade
        };

        popup.Opened += (s, args) =>
        {
            double squareWidth = squareHighlightedBox.ActualWidth > 0 ? squareHighlightedBox.ActualWidth : 40;
            double tooltipWidth = border.ActualWidth > 0 ? border.ActualWidth : stack.ActualWidth;

            if (toolTipColumn <= 4)
            {
                popup.HorizontalOffset = 0;
                arrow.HorizontalAlignment = HorizontalAlignment.Left;
                arrow.Margin = new Thickness(squareWidth / 2, -1, 0, 0);
            }
            else
            {
                popup.HorizontalOffset = squareWidth - tooltipWidth;
                arrow.HorizontalAlignment = HorizontalAlignment.Right;
                arrow.Margin = new Thickness(0, -1, squareWidth / 2, 0);
            }
        };

        _stepPopup = popup;

        Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            new Action(() =>
            {
                _stepPopup.IsOpen = true;
                SetHighlighting(solveStep); // Highlight cells after popup is open
            }));

        return squareHighlightedBox;
    }

    /// <summary>
    /// Handler for clicking outside the grid. Removes the tooltips
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainWindow_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_stepPopup != null && _stepPopup.IsOpen)
        {
            var clickedElement = e.OriginalSource as DependencyObject;
            bool clickedInsideGrid = false;

            while (clickedElement != null)
            {
                if (clickedElement == SudokuGrid)
                {
                    clickedInsideGrid = true;
                    break;
                }
                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            if (!clickedInsideGrid)
            {
                _stepPopup.IsOpen = false;
                _stepPopup = null;
                ClearHighlighting();
            }
        }
    }

    private void NewPuzzle_Click(object sender, RoutedEventArgs e)
    {
        _isNew = true;
        GridHelper.ClearSquaresNewPuzzle(_squares);
    }
}
