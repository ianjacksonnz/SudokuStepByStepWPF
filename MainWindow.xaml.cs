using SudokuStepByStep.Common;
using SudokuStepByStep.Logic;
using SudokuStepByStep.Logic.Helpers;
using SudokuStepByStep.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SudokuStepByStep;

public partial class MainWindow : Window
{
    private Dictionary<string, int[,]> _puzzles = new Dictionary<string, int[,]>();
    private SudokuSquare[,] _squares = new SudokuSquare[9, 9];
    private readonly List<TextBox> _highlightedSquares = [];
    private int _hintRow = -1;
    private int _hintCol = -1;
    private int _hintNumber = -1;
    private Popup? _hintPopup = null!;
    private TextBox? _prevHintBox = null!;
    private Enums.SolvingRule? _currentHintMethod;
    private Enums.SquareGroupType? _currentGroupType;
    private bool _showingPossibleValues = false;

    // Tracks OnlyValue squares that have been hinted
    private List<(int row, int col)> _shownOnlyValueSquares = new();

    // Tracks NakedPairs that have already been hinted
    private List<(int row1, int col1, int row2, int col2)> _hintedNakedPairs = new();
    private List<HashSet<(int row, int col)>> _hintedGroups = new();
    

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
                tb.TextChanged += (s, e) => UpdateCandidates();
                tb.LostFocus += (s, e) => ClearHighlighting();

                _squares[row, col] = new SudokuSquare(tb, candidates, border);

                SudokuGrid.Children.Add(border);
            }
        }
    }

    private void PuzzleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ResetHintTracking();

        if (PuzzleSelector.SelectedItem == null) { return; }

        string? selected = PuzzleSelector.SelectedItem.ToString();

        if (selected != null && _puzzles.TryGetValue(selected, out int[,]? value))
        {
            LoadPuzzle(value);
        }
    }

    private void ResetHintTracking()
    {
        _hintedNakedPairs.Clear();
        _shownOnlyValueSquares.Clear();
    }

    private void LoadPuzzle(int[,] puzzle)
    {
        PuzzleLoader.LoadPuzzle(puzzle, _squares);

        ClearHighlighting();
        UpdateCandidates();
    }

    private void Step_Click(object sender, RoutedEventArgs e)
    {
        if (GridHelper.PuzzleSolved(_squares))
        {
            MessageBox.Show("Puzzle Solved!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
            GridHelper.ShowPossibleValues(_squares, false);
            return;
        }

        GridHelper.ShowPossibleValues(_squares, true);
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        GridHelper.ShowPossibleValues(_squares, false);
        ResetHintTracking();

        // Clear the hinted pairs/groups tracking
        _hintedGroups.Clear();        // for both naked pairs and pointing pairs
        _hintedNakedPairs.Clear();    // if still used separately
        _prevHintBox = null;          // remove reference to previous hint textbox

        // Close any active hint popup
        if (_hintPopup != null)
        {
            _hintPopup.IsOpen = false;
            _hintPopup.Child = null;
            _hintPopup = null;
        }

        // Reset all hint state variables
        _currentHintMethod = null;
        _currentGroupType = null;
        _hintNumber = 0;
        _hintRow = -1;
        _hintCol = -1;

        GridHelper.ClearSquares(_squares);
        UpdateCandidates();
    }

    private void Solve_Click(object sender, RoutedEventArgs e)
    {
        // Get the numbers from the UI
        int[,] initial = GridHelper.GetNumbers(_squares);

        // Convert to candidate grid
        var candidateGrid = RulesHelper.InitializeBoard(initial);

        // Solve using candidate grid
        if (RulesHelper.SolveCompletePuzzle(candidateGrid))
        {
            // Update the UI with solved values
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (candidateGrid[r, c] != null && candidateGrid[r, c].Count == 1)
                    {
                        _squares[r, c].Box.Text = candidateGrid[r, c].First().ToString();
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("No solution found!", "Sudoku Solver",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        UpdateCandidates();
    }


    private void ShowPossibleValues_Click(object sender, RoutedEventArgs e)
    {
        _showingPossibleValues = !_showingPossibleValues; // toggle

        // Change button text accordingly
        var button = sender as Button;

        if (button != null)
        {
            button.Content = _showingPossibleValues ? "Hide Possible Values" : "Show Possible Values";
        }

        GridHelper.ShowPossibleValues(_squares, _showingPossibleValues);
    }



    private void UpdateCandidates()
    {
        if (!_showingPossibleValues) return; // Only update display if toggled on

        int[,] grid = GridHelper.GetNumbers(_squares);

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var square = _squares[r, c];

                if (grid[r, c] == 0)
                {
                    var possibleNumbers = RulesHelper.GetPossibleNumbers(grid, r, c);
                    square.CandidatesBlock.Text = string.Join(" ", possibleNumbers);
                    square.CandidatesBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    square.CandidatesBlock.Text = string.Empty;
                    square.CandidatesBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    //private void UpdateCandidates()
    //{
    //    if (!_showingPossibleValues) return; // Only update display if toggled on

    //    // Step 1: Get numbers from UI
    //    int[,] initial = GridHelper.GetNumbers(_squares);

    //    // Step 2: Convert to candidate grid
    //    HashSet<int>[,] candidateGrid = RulesHelper.InitializeBoard(initial);

    //    // Step 3: Update candidate display
    //    for (int r = 0; r < 9; r++)
    //    {
    //        for (int c = 0; c < 9; c++)
    //        {
    //            var square = _squares[r, c];

    //            if (initial[r, c] == 0) // unsolved square
    //            {
    //                // Show remaining candidates
    //                var candidates = candidateGrid[r, c];
    //                square.CandidatesBlock.Text = string.Join(" ", candidates);
    //                square.CandidatesBlock.Visibility = Visibility.Visible;
    //            }
    //            else
    //            {
    //                // Hide candidates for solved squares
    //                square.CandidatesBlock.Text = string.Empty;
    //                square.CandidatesBlock.Visibility = Visibility.Collapsed;
    //            }
    //        }
    //    }
    //}

    //private void Hint_Click(object sender, RoutedEventArgs e)
    //{
    //    if (GridHelper.PuzzleSolved(_squares))
    //    {
    //        MessageBox.Show("Puzzle Solved!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
    //        return;
    //    }

    //    int[,] grid = GridHelper.GetNumbers(_squares);
    //    var candidatesBoard = new HashSet<int>[9, 9];

    //    // Populate candidate sets for empty squares
    //    for (int r = 0; r < 9; r++)
    //    {
    //        for (int c = 0; c < 9; c++)
    //        {
    //            candidatesBoard[r, c] = grid[r, c] == 0
    //                ? new HashSet<int>(RulesHelper.GetPossibleNumbers(grid, r, c))
    //                : new HashSet<int>();
    //        }
    //    }

    //    // Detach previous handlers and clear previous highlighting/popup
    //    if (_prevHintBox != null)
    //    {
    //        _prevHintBox.KeyDown -= HintSquare_KeyDown;
    //        _prevHintBox.TextChanged -= HintSquare_TextChanged;
    //        _prevHintBox = null;
    //    }

    //    ClearHighlighting();

    //    bool isMultiHint = false;
    //    int hintNumber = 0;
    //    int hintRow = -1, hintCol = -1;
    //    Enums.SquareGroupType? groupType = null;
    //    int[] pairValues = Array.Empty<int>();
    //    (int row, int col)[] pairSquares = Array.Empty<(int, int)>();
    //    string explanation = null;

    //    // --- OnlyValue hint ---
    //    if (SolvingRules.OnlyValue(candidatesBoard, out hintNumber, out hintRow, out hintCol, out groupType))
    //    {
    //        _currentHintMethod = Enums.SolvingRule.OnlyValue;
    //        _currentGroupType = groupType;
    //        _hintNumber = hintNumber;
    //        _hintRow = hintRow;
    //        _hintCol = hintCol;
    //        isMultiHint = false;

    //        explanation = $"Number {hintNumber} can only go in this square in its {groupType?.ToString().ToLower()}.";
    //    }
    //    // --- Pairs hint (Naked + Pointing) ---
    //    else
    //    {
    //        bool pairFound = false;

    //        // First try NakedPairs
    //        if (SolvingRules.NakedPairs(candidatesBoard, _hintedGroups, out pairValues, out pairSquares, out groupType))
    //        {
    //            _currentHintMethod = Enums.SolvingRule.NakedPairs;
    //            pairFound = true;
    //        }
    //        // If no NakedPair, then try PointingPairs
    //        else if (SolvingRules.ApplyPointingPairs(candidatesBoard, _hintedGroups, out pairValues, out pairSquares, out groupType))
    //        {
    //            _currentHintMethod = Enums.SolvingRule.PointingPairs;
    //            pairFound = true;
    //        }

    //        if (pairFound)
    //        {
    //            // Check if this group was already hinted
    //            if (!_hintedGroups.Any(g => g.SetEquals(pairSquares)))
    //            {
    //                var groupSquares = new HashSet<(int row, int col)>(pairSquares);
    //                _hintedGroups.Add(groupSquares);

    //                _currentGroupType = groupType;
    //                isMultiHint = true;

    //                string squarePositions = string.Join(" and ", pairSquares.Select(p => $"({p.row + 1},{p.col + 1})"));
    //                explanation = $"{_currentHintMethod}: {{{string.Join(", ", pairValues)}}} found at {squarePositions}.";

    //                // 🔑 Refresh displayed candidates so removals are visible in the UI
    //                UpdateCandidates(); // TODO this does not include the candidates removes from the pointing pairs rule. It just has all possible values. Fix this
    //            }
    //            else
    //            {
    //                pairFound = false; // Already hinted, so skip
    //            }
    //        }

    //        if (!pairFound)
    //        {
    //            MessageBox.Show("No hints available!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
    //            return;
    //        }
    //    }

    //    // Determine placement target
    //    TextBox placementTargetBox = isMultiHint
    //        ? _squares[pairSquares[0].row, pairSquares[0].col].Box
    //        : _squares[hintRow, hintCol].Box;

    //    // --- Build popup and highlight logic ---
    //    var stack = new StackPanel { Orientation = Orientation.Vertical };

    //    var border = new Border
    //    {
    //        Background = Brushes.LightYellow,
    //        BorderBrush = Brushes.Gray,
    //        BorderThickness = new Thickness(1),
    //        CornerRadius = new CornerRadius(4),
    //        Padding = new Thickness(6),
    //        MinWidth = placementTargetBox.ActualWidth > 0 ? placementTargetBox.ActualWidth : 40,
    //        Child = new TextBlock
    //        {
    //            Text = explanation ?? "Hint",
    //            Foreground = Brushes.Black,
    //            FontSize = 11,
    //            TextAlignment = TextAlignment.Center,
    //            TextWrapping = TextWrapping.Wrap
    //        }
    //    };

    //    stack.Children.Add(border);

    //    var arrow = new System.Windows.Shapes.Polygon
    //    {
    //        Points = new PointCollection { new Point(0, 0), new Point(12, 0), new Point(6, 8) },
    //        Fill = Brushes.LightYellow,
    //        Stroke = Brushes.Gray,
    //        StrokeThickness = 1,
    //        Margin = new Thickness(0, -1, 0, 0),
    //        Width = 12,
    //        Height = 8
    //    };

    //    stack.Children.Add(arrow);

    //    var popup = new Popup
    //    {
    //        Child = stack,
    //        PlacementTarget = placementTargetBox,
    //        Placement = PlacementMode.Top,
    //        StaysOpen = true,
    //        AllowsTransparency = true,
    //        PopupAnimation = PopupAnimation.Fade
    //    };

    //    popup.Opened += (s, args) =>
    //    {
    //        int colToCheck = isMultiHint ? pairSquares[0].col : hintCol;

    //        if (colToCheck > 4)
    //        {
    //            double tooltipWidth = border.ActualWidth > 0 ? border.ActualWidth : stack.ActualWidth;
    //            double squareWidth = placementTargetBox.ActualWidth > 0 ? placementTargetBox.ActualWidth : 40;
    //            popup.HorizontalOffset = squareWidth - tooltipWidth;
    //        }
    //    };

    //    _hintPopup = popup;

    //    Application.Current.Dispatcher.BeginInvoke(
    //        DispatcherPriority.Background,
    //        new Action(() =>
    //        {
    //            _hintPopup.IsOpen = true;

    //            if (!isMultiHint)
    //            {
    //                HighlightOnlyValueSquare(_hintRow, _hintCol, _currentGroupType.Value);
    //            }
    //            else
    //            {
    //                HighlightNakedPair(pairSquares, _currentGroupType.Value);
    //            }
    //        }));

    //    _prevHintBox = placementTargetBox;
    //    _prevHintBox.KeyDown += HintSquare_KeyDown;
    //    _prevHintBox.TextChanged += HintSquare_TextChanged;
    //    _prevHintBox.Focus();
    //}

    private void HighlightOnlyValueSquare(int hintRow, int hintCol, Enums.SquareGroupType groupType)
    {
        switch (groupType)
        {
            case Enums.SquareGroupType.Row:
                for (int c = 0; c < 9; c++)
                {
                    var box = _squares[hintRow, c].Box;
                    box.Background = (c == hintCol) ? Brushes.LightGreen : Brushes.LightYellow;
                    _highlightedSquares.Add(box);
                }
                break;

            case Enums.SquareGroupType.Column:
                for (int r = 0; r < 9; r++)
                {
                    var box = _squares[r, hintCol].Box;
                    box.Background = (r == hintRow) ? Brushes.LightGreen : Brushes.LightYellow;
                    _highlightedSquares.Add(box);
                }
                break;

            case Enums.SquareGroupType.Grid:
                int startRow = (hintRow / 3) * 3;
                int startCol = (hintCol / 3) * 3;
                for (int r = startRow; r < startRow + 3; r++)
                {
                    for (int c = startCol; c < startCol + 3; c++)
                    {
                        var box = _squares[r, c].Box;
                        box.Background = (r == hintRow && c == hintCol) ? Brushes.LightGreen : Brushes.LightYellow;
                        _highlightedSquares.Add(box);
                    }
                }
                break;
        }
    }

    // Highlight a naked pair hint
    private void HighlightNakedPair((int row, int col)[] pairSquares, Enums.SquareGroupType groupType)
    {
        if (pairSquares.Length < 2) return;

        if (groupType == Enums.SquareGroupType.Row)
        {
            int r = pairSquares[0].row;
            for (int c = 0; c < 9; c++)
            {
                bool isPairSquare = pairSquares.Any(p => p.row == r && p.col == c);
                var box = _squares[r, c].Box;
                box.Background = isPairSquare ? Brushes.LightGreen : Brushes.LightYellow;
                _highlightedSquares.Add(box);
            }
        }
        else if (groupType == Enums.SquareGroupType.Column)
        {
            int c = pairSquares[0].col;
            for (int r = 0; r < 9; r++)
            {
                bool isPairSquare = pairSquares.Any(p => p.row == r && p.col == c);
                var box = _squares[r, c].Box;
                box.Background = isPairSquare ? Brushes.LightGreen : Brushes.LightYellow;
                _highlightedSquares.Add(box);
            }
        }
        else if (groupType == Enums.SquareGroupType.Grid)
        {
            int gridStartRow = (pairSquares[0].row / 3) * 3;
            int gridStartCol = (pairSquares[0].col / 3) * 3;
            for (int r = gridStartRow; r < gridStartRow + 3; r++)
            {
                for (int c = gridStartCol; c < gridStartCol + 3; c++)
                {
                    bool isPairSquare = pairSquares.Any(p => p.row == r && p.col == c);
                    var box = _squares[r, c].Box;
                    box.Background = isPairSquare ? Brushes.LightGreen : Brushes.LightYellow;
                    _highlightedSquares.Add(box);
                }
            }
        }
        else
        {
            // fallback: just highlight the pair squares
            foreach (var pc in pairSquares)
            {
                var box = _squares[pc.row, pc.col].Box;
                box.Background = Brushes.LightGreen;
                _highlightedSquares.Add(box);
            }
        }
    }

    /// <summary>
    /// Manually entering the hint number into the hint square
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HintSquare_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_hintRow < 0 || _hintCol < 0) return;

        var square = _squares[_hintRow, _hintCol];
        if (square.Box.Text == _hintNumber.ToString())
        {
            square.Box.TextChanged -= HintSquare_TextChanged;

            if (_hintPopup != null)
            {
                _hintPopup.IsOpen = false;
                _hintPopup = null;
            }

            ClearHighlighting();
            UpdateCandidates();

            // Move to next hint
            // Hint_Click(HintButton, new RoutedEventArgs(Button.ClickEvent));
        }
    }

    /// <summary>
    /// Clicking enter key for the hint square
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HintSquare_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter && _hintRow >= 0 && _hintCol >= 0)
        {
            var square = _squares[_hintRow, _hintCol];

            if (_prevHintBox != null)
            {
                _prevHintBox.KeyDown -= HintSquare_KeyDown;
                _prevHintBox.TextChanged -= HintSquare_TextChanged;
                _prevHintBox = null;
            }

            square.Box.Text = _hintNumber.ToString();

            if (_hintPopup != null)
            {
                _hintPopup.IsOpen = false;
                _hintPopup = null;
            }

            ClearHighlighting();
            UpdateCandidates();

            // Move to next hint
            // Hint_Click(HintButton, new RoutedEventArgs(Button.ClickEvent));
        }
    }

    private void ClearHighlighting()
    {
        foreach (var square in _highlightedSquares)
        {
            square.Background = Brushes.White;
        }

        _highlightedSquares.Clear();

        if (_hintPopup != null)
        {
            _hintPopup.IsOpen = false;
        }
    }

    /// <summary>
    /// Handler for clicking outside the grid. Removes the tooltips
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainWindow_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_hintPopup != null && _hintPopup.IsOpen)
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
                _hintPopup.IsOpen = false;
                _hintPopup = null;
                ClearHighlighting();
            }
        }
    }
}
