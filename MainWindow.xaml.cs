using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SudokuWpfApp
{
    public partial class MainWindow : Window
    {
        private class SudokuCell
        {
            public TextBox Box { get; set; }
            public TextBlock CandidatesBlock { get; set; }
            public Border Border { get; set; } // Add reference to Border
        }

        private SudokuCell[,] _cells = new SudokuCell[9, 9];
        private SudokuSolver _solver = new SudokuSolver();

        private readonly Dictionary<string, int[,]> _puzzles = new Dictionary<string, int[,]>
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
                    tb.GotFocus += (s, e) => HighlightPeers(r, c);
                    tb.LostFocus += (s, e) => ClearHighlighting();

                    _cells[row, col] = new SudokuCell { Box = tb, CandidatesBlock = candidates, Border = border };
                    SudokuGrid.Children.Add(border);
                }
            }
        }


        private void LoadPuzzle(int[,] puzzle)
        {
            for (int r = 0; r < 9; r++)
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

            ClearHighlighting();
            UpdateCandidates();
        }

        private void PuzzleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PuzzleSelector.SelectedItem == null) return;

            string selected = PuzzleSelector.SelectedItem.ToString();

            if (_puzzles.ContainsKey(selected))
                LoadPuzzle(_puzzles[selected]);
        }

        private int[,] GetBoard()
        {
            int[,] board = new int[9, 9];

            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    board[r, c] = int.TryParse(_cells[r, c].Box.Text, out int val) ? val : 0;

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

        private List<int> GetPossibleNumbers(int[,] board, int row, int col)
        {
            List<int> possible = new List<int>();

            for (int num = 1; num <= 9; num++)
                if (_solver.IsSafe(board, row, col, num))
                    possible.Add(num);
            return possible;
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            int[,] board = GetBoard();

            if (_solver.Solve(board))
            {
                for (int r = 0; r < 9; r++)
                    for (int c = 0; c < 9; c++)
                        _cells[r, c].Box.Text = board[r, c].ToString();
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
                for (int c = 0; c < 9; c++)
                    if (!_cells[r, c].Box.IsReadOnly)
                        _cells[r, c].Box.Text = "";

            UpdateCandidates();
        }

        private int _hintRow = -1, _hintCol = -1; // Track last hint cell
        private Popup _hintPopup = null; // Popup for hint

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            int[,] board = GetBoard();
            int minCandidates = 10;
            int hintRow = -1, hintCol = -1;
            List<int> hintCandidates = null;

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                    {
                        var possible = GetPossibleNumbers(board, r, c);
                        if (possible.Count < minCandidates)
                        {
                            minCandidates = possible.Count;
                            hintRow = r;
                            hintCol = c;
                            hintCandidates = possible;
                        }
                    }
                }
            }

            // Remove previous popup
            if (_hintPopup != null)
            {
                _hintPopup.IsOpen = false;
                _hintPopup = null;
            }

            if (hintRow >= 0 && hintCandidates != null)
            {
                _cells[hintRow, hintCol].Box.Background = Brushes.LightGreen;
                _cells[hintRow, hintCol].Box.Focus();
                string candidatesStr = string.Join(", ", hintCandidates);

                // Tooltip-like UI
                var stack = new StackPanel { Orientation = Orientation.Vertical };
                var border = new Border
                {
                    Background = Brushes.LightYellow,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6),
                    Child = new TextBlock
                    {
                        Text = $"Candidates: {candidatesStr}",
                        Foreground = Brushes.Black,
                        FontSize = 11
                    }
                };
                stack.Children.Add(border);
                // Arrow pointing down
                var arrow = new System.Windows.Shapes.Polygon
                {
                    Points = new PointCollection { new Point(0,0), new Point(12,0), new Point(6,8) },
                    Fill = Brushes.LightYellow,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, -1, 0, 0)
                };
                stack.Children.Add(arrow);

                _hintPopup = new Popup
                {
                    Child = stack,
                    PlacementTarget = _cells[hintRow, hintCol].Border,
                    Placement = PlacementMode.Top,
                    StaysOpen = false,
                    IsOpen = true,
                    AllowsTransparency = true
                };
            }
            else
            {
                MessageBox.Show("No hints available. Puzzle may be complete or need advanced solving.",
                                "Sudoku Hint", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void HighlightPeers(int focusRow, int focusCol)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (_cells[r, c].Box.Background != Brushes.LightGreen)
                        _cells[r, c].Box.Background = Brushes.White;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                _cells[focusRow, i].Box.Background = Brushes.LightYellow;
                _cells[i, focusCol].Box.Background = Brushes.LightYellow;
            }

            int startRow = focusRow - focusRow % 3;
            int startCol = focusCol - focusCol % 3;

            for (int r = startRow; r < startRow + 3; r++)
                for (int c = startCol; c < startCol + 3; c++)
                    _cells[r, c].Box.Background = Brushes.LightYellow;

            _cells[focusRow, focusCol].Box.Background = Brushes.LightBlue;
        }

        private void ClearHighlighting()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    if (_cells[r, c].Box.Background != Brushes.LightGreen)
                        _cells[r, c].Box.Background = Brushes.White;
                }
            if (_hintPopup != null)
            {
                _hintPopup.IsOpen = false;
                _hintPopup = null;
            }
        }
    }
}
