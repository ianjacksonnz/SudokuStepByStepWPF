using SudokuHelper.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace SudokuHelper;

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
    private Enums.SolvingMethod _currentHintMethod;
    private Enums.CellGroupType? _currentGroupType;


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



    private void InitializeGrid()
    {
        SudokuGrid.Children.Clear();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                // Use a Border for cell borders
                var border = new Border
                {
                    BorderThickness = new Thickness(
                        col % 3 == 0 ? 2 : 0.5,
                        row % 3 == 0 ? 2 : 0.5,
                        (col + 1) % 3 == 0 ? 2 : 0.5,
                        (row + 1) % 3 == 0 ? 2 : 0.5),
                    BorderBrush = Brushes.Black
                };

                // Inner Grid for TextBox + Candidates
                var grid = new Grid();

                var tb = new TextBox
                {
                    FontSize = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Brushes.White
                };

                var candidates = new TextBlock
                {
                    FontSize = 10,
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                };

                grid.Children.Add(candidates);
                grid.Children.Add(tb);

                border.Child = grid;

                int r = row, c = col;
                tb.TextChanged += (s, e) => UpdateCandidates();
                tb.LostFocus += (s, e) => ClearHighlighting();

                _cells[row, col] = new SudokuCell { Box = tb, CandidatesBlock = candidates, Border = border };
                SudokuGrid.Children.Add(border);
            }
        }
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

    private void PuzzleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PuzzleSelector.SelectedItem == null) { return; }

        string? selected = PuzzleSelector.SelectedItem.ToString();

        if (selected != null && _puzzles.TryGetValue(selected, out int[,]? value))
        {
            LoadPuzzle(value);
        }
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

    private void UpdateCandidates()
    {
        int[,] board = GetBoard();

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (_cells[r, c].Box.IsReadOnly || !string.IsNullOrEmpty(_cells[r, c].Box.Text))
                {
                    _cells[r, c].CandidatesBlock.Text = "";
                    continue;
                }

                var possible = GetPossibleNumbers(board, r, c);
                _cells[r, c].CandidatesBlock.Text = string.Join(" ", possible);
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

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (!_cells[r, c].Box.IsReadOnly)
                {
                    _cells[r, c].Box.Text = "";
                }
            }
        }

        UpdateCandidates();
    }


    private void Hint_Click(object sender, RoutedEventArgs e)
    {
        int[,] board = GetBoard();
        var candidatesBoard = new HashSet<int>[9, 9];

        // Populate candidate sets for empty cells
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                candidatesBoard[r, c] = board[r, c] == 0 ? [.. GetPossibleNumbers(board, r, c)] : [];
            }
        }

        int hintRow = -1, hintCol = -1;
        string? explanation = null;

        // Remove previous handlers and highlighting
        if (_prevHintBox != null)
        {
            _prevHintBox.KeyDown -= HintCell_KeyDown;
            _prevHintBox.TextChanged -= HintCell_TextChanged;
            _prevHintBox = null;
        }
        ClearHighlighting();

        Enums.CellGroupType? groupType;

        // Determine the next hint
        if (SolvingRules.OnlyValue(candidatesBoard, out int hintNumber, out hintRow, out hintCol, out groupType))
        {
            _currentHintMethod = Enums.SolvingMethod.OnlyValue;
            _currentGroupType = groupType;

            explanation = $"Number {hintNumber} can only go in this cell in its {groupType?.ToString().ToLower()}.";
            _hintNumber = hintNumber;
        }
        else if (SolvingRules.NakedPairs(candidatesBoard, out hintNumber, out hintRow, out hintCol, out groupType))
        {
            _currentHintMethod = Enums.SolvingMethod.NakedPairs;
            _currentGroupType = groupType;

            explanation = $"Naked pair found, number {hintNumber} can be placed here in its {groupType?.ToString().ToLower()}.";
            _hintNumber = hintNumber;
        }
        else
        {
            MessageBox.Show("No hints available!", "Sudoku Solver", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _hintRow = hintRow;
        _hintCol = hintCol;

        if (_hintRow < 0 || _hintCol < 0) return;

        var cell = _cells[_hintRow, _hintCol];
        var stack = new StackPanel { Orientation = Orientation.Vertical };

        var border = new Border
        {
            Background = Brushes.LightYellow,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(6),
            MinWidth = cell.Box.ActualWidth > 0 ? cell.Box.ActualWidth : 40,
            Child = new TextBlock
            {
                Text = explanation ?? "Hint: " + cell.CandidatesBlock.Text,
                Foreground = Brushes.Black,
                FontSize = 11,
                TextAlignment = TextAlignment.Center
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

        // Align tooltip left/right depending on cell column
        if (_hintCol <= 4)
        {
            stack.HorizontalAlignment = HorizontalAlignment.Left;
            border.HorizontalAlignment = HorizontalAlignment.Left;
            arrow.HorizontalAlignment = HorizontalAlignment.Left;
        }
        else
        {
            stack.HorizontalAlignment = HorizontalAlignment.Right;
            border.HorizontalAlignment = HorizontalAlignment.Right;
            arrow.HorizontalAlignment = HorizontalAlignment.Right;
        }

        var popup = new Popup
        {
            Child = stack,
            PlacementTarget = cell.Box,
            Placement = PlacementMode.Top,
            StaysOpen = true,
            AllowsTransparency = true,
            PopupAnimation = PopupAnimation.Fade
        };

        popup.Opened += (s, args) =>
        {
            if (_hintCol > 4)
            {
                double tooltipWidth = border.ActualWidth > 0 ? border.ActualWidth : stack.ActualWidth;
                double cellWidth = cell.Box.ActualWidth > 0 ? cell.Box.ActualWidth : 40;
                popup.HorizontalOffset = cellWidth - tooltipWidth;
            }
        };

        _hintPopup = popup;

        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                _hintPopup.IsOpen = true;

                // Highlight cells based on OnlyValue rule group type
                if (_currentHintMethod == Enums.SolvingMethod.OnlyValue && _currentGroupType.HasValue)
                {
                    switch (_currentGroupType.Value)
                    {
                        case Enums.CellGroupType.Column:
                            for (int r = 0; r < 9; r++)
                            {
                                var colCell = _cells[r, _hintCol].Box;
                                colCell.Background = (r == _hintRow) ? Brushes.LightGreen : Brushes.LightYellow;
                                _highlightedCells.Add(colCell);
                            }
                            break;

                        case Enums.CellGroupType.Row:
                            for (int c = 0; c < 9; c++)
                            {
                                var rowCell = _cells[_hintRow, c].Box;
                                rowCell.Background = (c == _hintCol) ? Brushes.LightGreen : Brushes.LightYellow;
                                _highlightedCells.Add(rowCell);
                            }
                            break;

                        case Enums.CellGroupType.Grid:
                            int startRow = (_hintRow / 3) * 3;
                            int startCol = (_hintCol / 3) * 3;
                            for (int r = startRow; r < startRow + 3; r++)
                            {
                                for (int c = startCol; c < startCol + 3; c++)
                                {
                                    var gridCell = _cells[r, c].Box;
                                    gridCell.Background = (r == _hintRow && c == _hintCol) ? Brushes.LightGreen : Brushes.LightYellow;
                                    _highlightedCells.Add(gridCell);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    // For NakedPairs or other hints, just highlight the hint cell
                    cell.Box.Background = Brushes.LightGreen;
                    _highlightedCells.Add(cell.Box);
                }
            }));

        // Set focus so Enter advances automatically
        _prevHintBox = cell.Box;
        _prevHintBox.KeyDown += HintCell_KeyDown;
        _prevHintBox.TextChanged += HintCell_TextChanged;
        _prevHintBox.Focus();
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
            Hint_Click(HintButton, new RoutedEventArgs(Button.ClickEvent));
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
            Hint_Click(HintButton, new RoutedEventArgs(Button.ClickEvent));
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


}
