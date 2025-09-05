using SudokoStepByStep;
using SudokoStepByStep.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace SudokuStepByStep;

public partial class MainWindow : Window
{
    private class SudokuCell
    {
        public required TextBox Box { get; set; }
        public required TextBlock CandidatesBlock { get; set; }
        public required Border Border { get; set; } // Add reference to Border
    }

    private static readonly SudokuCell[,] sudokuCells = new SudokuCell[9, 9];
    private readonly SudokuCell[,] _cells = sudokuCells;
    private readonly SudokuSolver _solver = new();
    private readonly List<TextBox> _highlightedCells = [];
    private int _hintRow = -1;
    private int _hintCol = -1;
    private int _hintNumber = -1;
    private Popup? _hintPopup = null!;
    private TextBox? _prevHintBox = null!;
    private Enums.SolvingMethod? _currentHintMethod;
    private Enums.CellGroupType? _currentGroupType;
    private bool _showingPossibleValues = false;

    // Tracks OnlyValue cells that have been hinted
    private List<(int row, int col)> _shownOnlyValueCells = new();

    // Tracks NakedPairs that have already been hinted
    private List<(int row1, int col1, int row2, int col2)> _hintedNakedPairs = new();
    private List<HashSet<(int row, int col)>> _hintedGroups = new();

    private readonly Dictionary<string, int[,]> _puzzles = new()
    {
        { "Genius Puzzle 1", new int[9,9]
            {
                {0,0,0,0,0,0,0,0,4},
                {0,0,9,8,7,0,0,0,6},
                {0,0,0,0,0,3,0,7,0},
                {0,0,0,0,0,8,2,4,0},
                {0,0,5,0,9,0,8,0,0},
                {0,1,2,4,0,0,0,0,0},
                {0,2,0,7,0,0,0,0,0},
                {3,0,0,0,6,1,5,0,0},
                {7,0,0,0,0,0,0,0,0}
            }
        },
        { "Genius Puzzle 2", new int[9,9]
            {
                {0,0,0,0,0,0,0,0,0},
                {3,0,0,8,0,0,0,4,0},
                {0,1,2,7,0,0,0,6,5},
                {0,4,0,9,0,6,5,0,0},
                {0,0,1,0,8,0,7,0,0},
                {0,0,6,2,0,1,0,8,0},
                {1,5,0,0,0,4,9,2,0},
                {0,9,0,0,0,7,0,0,3},
                {0,0,0,0,0,0,0,0,0}
            }
        },
        { "Genius Puzzle 3", new int[9,9]
            {
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,7,5,3,8,0},
                {3,0,6,9,0,0,2,0,0},
                {0,0,0,0,0,1,0,0,9},
                {0,0,2,0,8,0,6,0,0},
                {4,0,0,2,0,0,0,0,0},
                {0,0,5,0,0,7,9,0,1},
                {0,2,3,6,9,0,0,0,0},
                {0,0,0,0,0,0,0,0,0}
            }
        },
        { "Genius Puzzle 4", new int[9,9]
            {
                {0,0,0,0,0,0,0,0,0},
                {0,3,0,6,9,0,0,8,5},
                {9,8,0,1,0,0,0,6,3},
                {0,0,1,0,0,0,0,0,4},
                {0,6,0,4,8,7,0,1,0},
                {3,0,0,0,0,0,6,0,0},
                {2,7,0,0,0,6,0,3,8},
                {8,1,0,0,5,2,0,9,0},
                {0,0,0,0,0,0,0,0,0}
            }
        },
        { "Genius Puzzle 5", new int[9,9]
            {
                {0,9,0,0,0,0,0,0,7},
                {0,0,3,0,8,0,9,0,0},
                {0,0,0,0,0,7,4,0,3},
                {9,0,0,0,5,0,8,0,0},
                {6,0,0,2,9,8,0,0,4},
                {0,0,5,0,6,0,0,0,2},
                {5,0,6,1,0,0,0,0,0},
                {0,0,8,0,4,0,6,0,0},
                {1,0,0,0,0,0,0,7,0}
            }
        },
        { "Genius Puzzle 6", new int[9,9]
            {
                {0,7,0,0,0,0,0,0,0},
                {0,0,1,2,0,0,0,0,9},
                {0,3,0,0,0,0,4,1,6},
                {9,0,0,0,4,0,0,0,0},
                {1,2,0,0,3,0,0,9,7},
                {0,0,0,0,1,0,0,0,2},
                {7,1,2,0,0,0,0,6,0},
                {8,0,0,0,0,3,2,0,0},
                {0,0,0,0,0,0,0,5,0}
            }
        },
        { "Genius Puzzle 7", new int[9,9]
            {
                {0,0,1,0,0,0,0,0,7},
                {0,0,0,8,5,0,1,0,0},
                {0,0,0,0,0,0,0,9,4},
                {0,1,0,5,0,0,7,6,0},
                {0,5,0,4,1,9,0,8,0},
                {0,9,3,0,0,6,0,1,0},
                {4,6,0,0,0,0,0,0,0},
                {0,0,5,0,3,7,0,0,0},
                {3,0,0,0,0,0,2,0,0}
            }
        },
        { "Genius Puzzle 8", new int[9,9]
            {
                {0,0,0,0,0,0,0,9,3},
                {0,0,0,0,0,0,0,8,0},
                {2,9,0,7,0,0,5,0,0},
                {6,0,5,9,0,3,0,0,4},
                {0,0,2,0,8,0,9,0,0},
                {3,0,0,4,0,7,2,0,8},
                {0,0,4,0,0,6,0,1,9},
                {0,5,0,0,0,0,0,0,0},
                {1,2,0,0,0,0,0,0,0}
            }
        },
        { "Genius Puzzle 9", new int[9,9]
            {
                {0,0,0,0,0,0,4,0,0},
                {7,0,0,5,4,0,0,9,3},
                {0,0,5,9,0,0,0,0,0},
                {0,0,0,0,0,2,6,0,8},
                {0,0,6,0,3,0,5,0,0},
                {2,0,7,8,0,0,0,0,0},
                {0,0,0,0,0,1,3,0,0},
                {5,6,0,0,9,3,0,0,2},
                {0,0,8,0,0,0,0,0,0}
            }
        },
        { "Genius Puzzle 10", new int[9,9]
            {
                {0,0,0,0,0,0,0,3,0},
                {0,0,0,0,1,0,0,0,9},
                {4,2,0,0,8,0,6,7,0},
                {0,0,0,8,0,1,2,0,3},
                {0,9,0,0,0,0,0,8,0},
                {3,0,8,5,0,2,0,0,0},
                {0,3,1,0,7,0,0,9,6},
                {7,0,0,0,2,0,0,0,0},
                {0,6,0,0,0,0,0,0,0}
            }
        }
    };


    public MainWindow()
    {
        InitializeComponent();
        InitializeGrid();

        foreach (var key in _puzzles.Keys)
            PuzzleSelector.Items.Add(key);

        PuzzleSelector.SelectedIndex = 0;

        // Close hint tooltip on outside clicks
        this.PreviewMouseDown += MainWindow_PreviewMouseDown;
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

                _cells[row, col] = new SudokuCell
                {
                    Box = tb,
                    CandidatesBlock = candidates,
                    Border = border
                };

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
        _shownOnlyValueCells.Clear();
    }

    private void LoadPuzzle(int[,] puzzle)
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (puzzle[r, c] != 0)
                {
                    _cells[r, c].Box.Text = puzzle[r, c].ToString();
                    _cells[r, c].Box.IsReadOnly = true;
                    _cells[r, c].Box.Foreground = Brushes.DarkBlue;
                    _cells[r, c].CandidatesBlock.Text = "";
                }
                else
                {
                    _cells[r, c].Box.Text = "";
                    _cells[r, c].Box.IsReadOnly = false;
                    _cells[r, c].Box.Foreground = Brushes.Black;
                }
            }
        }

        ClearHighlighting();
        UpdateCandidates();
    }



    private int[,] GetBoard()
    {
        int[,] board = new int[9, 9];

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                board[r, c] = int.TryParse(_cells[r, c].Box.Text, out int val) ? val : 0;
            }
        }

        return board;
    }



    private void Step_Click(object sender, RoutedEventArgs e)
    {
        ShowPossibleValues(true); 
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ShowPossibleValues(false);
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

        // --- Clear the board cells for editable cells ---
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (!_cells[r, c].Box.IsReadOnly)
                {
                    _cells[r, c].Box.Text = "";
                    _cells[r, c].Box.Background = Brushes.White; // reset background highlighting
                }
            }
        }

        // --- Recalculate candidates for all cells ---
        UpdateCandidates();
    }

    private void Solve_Click(object sender, RoutedEventArgs e)
    {
        int[,] board = GetBoard();

        if (SudokuSolver.Solve(board))
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    _cells[r, c].Box.Text = board[r, c].ToString();
                }
            }
        }
        else
        {
            MessageBox.Show("No solution found!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        UpdateCandidates();
    }

    private void UpdateCandidates()
    {
        if (!_showingPossibleValues) return; // Only update display if toggled on

        int[,] board = GetBoard();

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var cell = _cells[r, c];

                if (board[r, c] == 0)
                {
                    var possibleNumbers = GetPossibleNumbers(board, r, c);
                    cell.CandidatesBlock.Text = string.Join(" ", possibleNumbers);
                    cell.CandidatesBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    cell.CandidatesBlock.Text = string.Empty;
                    cell.CandidatesBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }


    private static List<int> GetPossibleNumbers(int[,] board, int row, int col)
    {
        List<int> possible = [];

        for (int num = 1; num <= 9; num++)
        {
            if (SudokuSolver.IsSafe(board, row, col, num))
            {
                possible.Add(num);
            }
        }

        return possible;
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

        ShowPossibleValues(_showingPossibleValues);
    }

    private void ShowPossibleValues(bool show)
    {
        int[,] board = GetBoard();

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var cell = _cells[r, c];

                if (show && board[r, c] == 0)
                {
                    var possibleNumbers = GetPossibleNumbers(board, r, c);
                    cell.CandidatesBlock.Text = string.Join(" ", possibleNumbers);
                    cell.CandidatesBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    cell.CandidatesBlock.Text = string.Empty;
                    cell.CandidatesBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    private void Hint_Click(object sender, RoutedEventArgs e)
    {
        if (PuzzleSolved())
        {
            MessageBox.Show("Puzzle Solved!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        int[,] board = GetBoard();
        var candidatesBoard = new HashSet<int>[9, 9];

        // Populate candidate sets for empty cells
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                candidatesBoard[r, c] = board[r, c] == 0
                    ? new HashSet<int>(GetPossibleNumbers(board, r, c))
                    : new HashSet<int>();
            }
        }

        // Detach previous handlers and clear previous highlighting/popup
        if (_prevHintBox != null)
        {
            _prevHintBox.KeyDown -= HintCell_KeyDown;
            _prevHintBox.TextChanged -= HintCell_TextChanged;
            _prevHintBox = null;
        }

        ClearHighlighting();

        bool isMultiHint = false;
        int hintNumber = 0;
        int hintRow = -1, hintCol = -1;
        Enums.CellGroupType? groupType = null;
        int[] pairValues = Array.Empty<int>();
        (int row, int col)[] pairCells = Array.Empty<(int, int)>();
        string explanation = null;

        // --- OnlyValue hint ---
        if (SolvingRules.OnlyValue(candidatesBoard, out hintNumber, out hintRow, out hintCol, out groupType))
        {
            _currentHintMethod = Enums.SolvingMethod.OnlyValue;
            _currentGroupType = groupType;
            _hintNumber = hintNumber;
            _hintRow = hintRow;
            _hintCol = hintCol;
            isMultiHint = false;

            explanation = $"Number {hintNumber} can only go in this cell in its {groupType?.ToString().ToLower()}.";
        }
        // --- Pairs hint (Naked + Pointing) ---
        else
        {
            bool pairFound = false;

            // First try NakedPairs
            if (SolvingRules.NakedPairs(candidatesBoard, _hintedGroups, out pairValues, out pairCells, out groupType))
            {
                _currentHintMethod = Enums.SolvingMethod.NakedPairs;
                pairFound = true;
            }
            // If no NakedPair, then try PointingPairs
            else if (SolvingRules.ApplyPointingPairs(candidatesBoard, _hintedGroups, out pairValues, out pairCells, out groupType))
            {
                _currentHintMethod = Enums.SolvingMethod.PointingPairs;
                pairFound = true;
            }

            if (pairFound)
            {
                // Check if this group was already hinted
                if (!_hintedGroups.Any(g => g.SetEquals(pairCells)))
                {
                    var groupCells = new HashSet<(int row, int col)>(pairCells);
                    _hintedGroups.Add(groupCells);

                    _currentGroupType = groupType;
                    isMultiHint = true;

                    string cellPositions = string.Join(" and ", pairCells.Select(p => $"({p.row + 1},{p.col + 1})"));
                    explanation = $"{_currentHintMethod}: {{{string.Join(", ", pairValues)}}} found at {cellPositions}.";

                    // 🔑 Refresh displayed candidates so removals are visible in the UI
                    UpdateCandidates(); // TODO this does not include the candidates removes from the pointing pairs rule. It just has all possible values. Fix this
                }
                else
                {
                    pairFound = false; // Already hinted, so skip
                }
            }

            if (!pairFound)
            {
                MessageBox.Show("No hints available!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        // Determine placement target
        TextBox placementTargetBox = isMultiHint
            ? _cells[pairCells[0].row, pairCells[0].col].Box
            : _cells[hintRow, hintCol].Box;

        // --- Build popup and highlight logic ---
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var border = new Border
        {
            Background = Brushes.LightYellow,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(6),
            MinWidth = placementTargetBox.ActualWidth > 0 ? placementTargetBox.ActualWidth : 40,
            Child = new TextBlock
            {
                Text = explanation ?? "Hint",
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
            PlacementTarget = placementTargetBox,
            Placement = PlacementMode.Top,
            StaysOpen = true,
            AllowsTransparency = true,
            PopupAnimation = PopupAnimation.Fade
        };

        popup.Opened += (s, args) =>
        {
            int colToCheck = isMultiHint ? pairCells[0].col : hintCol;

            if (colToCheck > 4)
            {
                double tooltipWidth = border.ActualWidth > 0 ? border.ActualWidth : stack.ActualWidth;
                double cellWidth = placementTargetBox.ActualWidth > 0 ? placementTargetBox.ActualWidth : 40;
                popup.HorizontalOffset = cellWidth - tooltipWidth;
            }
        };

        _hintPopup = popup;

        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                _hintPopup.IsOpen = true;

                if (!isMultiHint)
                {
                    HighlightOnlyValueCell(_hintRow, _hintCol, _currentGroupType.Value);
                }
                else
                {
                    HighlightNakedPair(pairCells, _currentGroupType.Value);
                }
            }));

        _prevHintBox = placementTargetBox;
        _prevHintBox.KeyDown += HintCell_KeyDown;
        _prevHintBox.TextChanged += HintCell_TextChanged;
        _prevHintBox.Focus();
    }

    private void HighlightOnlyValueCell(int hintRow, int hintCol, Enums.CellGroupType groupType)
    {
        switch (groupType)
        {
            case Enums.CellGroupType.Row:
                for (int c = 0; c < 9; c++)
                {
                    var box = _cells[hintRow, c].Box;
                    box.Background = (c == hintCol) ? Brushes.LightGreen : Brushes.LightYellow;
                    _highlightedCells.Add(box);
                }
                break;

            case Enums.CellGroupType.Column:
                for (int r = 0; r < 9; r++)
                {
                    var box = _cells[r, hintCol].Box;
                    box.Background = (r == hintRow) ? Brushes.LightGreen : Brushes.LightYellow;
                    _highlightedCells.Add(box);
                }
                break;

            case Enums.CellGroupType.Grid:
                int startRow = (hintRow / 3) * 3;
                int startCol = (hintCol / 3) * 3;
                for (int r = startRow; r < startRow + 3; r++)
                {
                    for (int c = startCol; c < startCol + 3; c++)
                    {
                        var box = _cells[r, c].Box;
                        box.Background = (r == hintRow && c == hintCol) ? Brushes.LightGreen : Brushes.LightYellow;
                        _highlightedCells.Add(box);
                    }
                }
                break;
        }
    }

    // Highlight a naked pair hint
    private void HighlightNakedPair((int row, int col)[] pairCells, Enums.CellGroupType groupType)
    {
        if (pairCells.Length < 2) return;

        if (groupType == Enums.CellGroupType.Row)
        {
            int r = pairCells[0].row;
            for (int c = 0; c < 9; c++)
            {
                bool isPairCell = pairCells.Any(p => p.row == r && p.col == c);
                var box = _cells[r, c].Box;
                box.Background = isPairCell ? Brushes.LightGreen : Brushes.LightYellow;
                _highlightedCells.Add(box);
            }
        }
        else if (groupType == Enums.CellGroupType.Column)
        {
            int c = pairCells[0].col;
            for (int r = 0; r < 9; r++)
            {
                bool isPairCell = pairCells.Any(p => p.row == r && p.col == c);
                var box = _cells[r, c].Box;
                box.Background = isPairCell ? Brushes.LightGreen : Brushes.LightYellow;
                _highlightedCells.Add(box);
            }
        }
        else if (groupType == Enums.CellGroupType.Grid)
        {
            int gridStartRow = (pairCells[0].row / 3) * 3;
            int gridStartCol = (pairCells[0].col / 3) * 3;
            for (int r = gridStartRow; r < gridStartRow + 3; r++)
            {
                for (int c = gridStartCol; c < gridStartCol + 3; c++)
                {
                    bool isPairCell = pairCells.Any(p => p.row == r && p.col == c);
                    var box = _cells[r, c].Box;
                    box.Background = isPairCell ? Brushes.LightGreen : Brushes.LightYellow;
                    _highlightedCells.Add(box);
                }
            }
        }
        else
        {
            // fallback: just highlight the pair cells
            foreach (var pc in pairCells)
            {
                var box = _cells[pc.row, pc.col].Box;
                box.Background = Brushes.LightGreen;
                _highlightedCells.Add(box);
            }
        }
    }


    private bool PuzzleSolved()
    {
        int[,] board = GetBoard();

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (board[r, c] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Manually entering the hint number into the hint cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HintCell_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_hintRow < 0 || _hintCol < 0) return;

        var cell = _cells[_hintRow, _hintCol];
        if (cell.Box.Text == _hintNumber.ToString())
        {
            cell.Box.TextChanged -= HintCell_TextChanged;

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
    /// Clicking enter key for the hint cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HintCell_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter && _hintRow >= 0 && _hintCol >= 0)
        {
            var cell = _cells[_hintRow, _hintCol];

            if (_prevHintBox != null)
            {
                _prevHintBox.KeyDown -= HintCell_KeyDown;
                _prevHintBox.TextChanged -= HintCell_TextChanged;
                _prevHintBox = null;
            }

            cell.Box.Text = _hintNumber.ToString();

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
        foreach (var cell in _highlightedCells)
        {
            cell.Background = Brushes.White;
        }

        _highlightedCells.Clear();

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
