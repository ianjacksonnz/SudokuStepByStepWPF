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
    private int _stepRow = -1;
    private int _stepCol = -1;
    private int _stepNumber = -1;
    private Popup? _stepPopup = null!;
    private TextBox? _prevStepBox = null!;
  

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

        PuzzleSelector.SelectedIndex = 0;
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
        // ResetHintTracking();

        if (PuzzleSelector.SelectedItem == null) { return; }

        string? selected = PuzzleSelector.SelectedItem.ToString();

        if (selected != null && _puzzles.TryGetValue(selected, out int[,]? value))
        {
            LoadPuzzle(value);
        }
    }

    private void LoadPuzzle(int[,] puzzle)
    {
        PuzzleLoader.LoadPuzzle(puzzle, _squares);

        ClearHighlighting();
        RulesHelper.SetPossibleNumbers(_squares, true);
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

        var solveStep = RulesEngine.CalculateNextStep(_squares);

        if ((solveStep != null) && (solveStep.Solved || solveStep.CandidatesRemoved))
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
        square.Number = solveStep.Number;
        square.PossibleNumbers.Clear();

        _stepRow = solveStep.Row;
        _stepCol = solveStep.Column;
        _stepNumber = solveStep.Number;

        if (solveStep.Solved)
        {
            var box = _squares[solveStep.Row, solveStep.Column].Box;
            box.Text = solveStep.Number.ToString();
            box.Background = Brushes.LightGreen;

            // RulesHelper.SetPossibleValues(_squares, true);
            RulesHelper.RemoveGridPossibleNumbersAfterStep(_squares, solveStep);
        }

        foreach (var (r, c) in solveStep.HighlightedSquares)
        {
            var highlightedSquare = _squares[r, c];
            highlightedSquare.CandidatesBlock.Text = string.Join(" ", highlightedSquare.PossibleNumbers);
            highlightedSquare.Box.Background = Brushes.LightYellow;
        }

        var squareHighlightedBox = SetToolTip(solveStep);

        // Remove focus from the solved cell and do not attach key handlers
        _prevStepBox = squareHighlightedBox;
        //_prevStepBox.KeyDown += StepSquare_KeyDown;
        //_prevStepBox.TextChanged += StepSquare_TextChanged;
        // Do not set focus
        Keyboard.ClearFocus();
    }

    private void SetHighlighting(SolveStep solveStep)
    {
        if (solveStep.HighlightedSquares.Count == 0)
        {
            var box = _squares[solveStep.Row, solveStep.Column].Box;
            box.Background = Brushes.LightGreen;
        }
        else
        {
            // Highlight solved cell
            var solvedBox = _squares[solveStep.Row, solveStep.Column].Box;
            solvedBox.Background = Brushes.LightGreen;

            // Highlight other squares
            foreach (var (r, c) in solveStep.HighlightedSquares)
            {
                var box = _squares[r, c].Box;
                box.Background = Brushes.LightYellow;
            }
        }
    }

    private TextBox SetToolTip(SolveStep solveStep)
    {
        TextBox squareHighlightedBox = _squares[solveStep.Row, solveStep.Column].Box;

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

            if (solveStep.Column <= 4)
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

    ///// <summary>
    ///// Clicking enter key for the Step square
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private void StepSquare_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    //{
    //    if (e.Key == System.Windows.Input.Key.Enter && _stepRow >= 0 && _stepCol >= 0)
    //    {
    //        var square = _squares[_stepRow, _stepCol];

    //        if (_prevStepBox != null)
    //        {
    //            _prevStepBox.KeyDown -= StepSquare_KeyDown;
    //            //_prevStepBox.TextChanged -= StepSquare_TextChanged;
    //            _prevStepBox = null;
    //        }

    //        square.Box.Text = _stepNumber.ToString();

    //        if (_stepPopup != null)
    //        {
    //            _stepPopup.IsOpen = false;
    //            _stepPopup = null;
    //        }

    //        ClearHighlighting();
    //        RulesHelper.SetPossibleValues(_squares, true);

    //        // Move to next step
    //        Step_Click(StepButton, new RoutedEventArgs(Button.ClickEvent));
    //    }
    //}

    ///// <summary>
    ///// Manually entering the step number into the step square
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private void StepSquare_TextChanged(object sender, TextChangedEventArgs e)
    //{
    //    if (_stepRow < 0 || _stepCol < 0) return;

    //    var square = _squares[_stepRow, _stepCol];

    //    if (square.Box.Text == _stepNumber.ToString())
    //    {
    //        square.Box.TextChanged -= StepSquare_TextChanged;

    //        if (_stepPopup != null)
    //        {
    //            _stepPopup.IsOpen = false;
    //            _stepPopup = null;
    //        }

    //        ClearHighlighting();
    //        GridHelper.SetPossibleValues(_squares, true);

    //        // Move to next step
    //        // Step_Click(StepButton, new RoutedEventArgs(Button.ClickEvent));
    //    }
    //}

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        // ResetHintTracking();
        GridHelper.ClearSquares(_squares);
        RulesHelper.SetPossibleNumbers(_squares, true);
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

}
