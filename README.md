
# Sudoku Step By Step WPF

A modern WPF application for learning and solving Sudoku puzzles step by step. This app demonstrates various Sudoku solving techniques interactively, making it ideal for both beginners and enthusiasts.

## Features

- **Step-by-step solving:** Visualize each logical step in solving a Sudoku puzzle.
- **Multiple solving techniques:** Includes Naked Single, Hidden Single, Naked Pairs, Pointing Pairs, Hidden Pairs, Naked Triples, Hidden Triples, Naked Quads, X-Wing, and more.
- **Puzzle selection:** Load and switch between different puzzles.
- **Candidate highlighting:** Shows possible numbers for each cell.
- **Explanations:** Displays explanations for each solving step.
- **Reset and clear:** Easily reset the puzzle or clear the grid.
- **Modern UI:** Built with WPF and .NET 8, supporting Windows 10/11.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 or later (recommended for development)

### Building and Running

1. **Clone the repository:** 
2. **Open the solution in Visual Studio.**
3. **Restore NuGet packages** (Visual Studio will prompt you).
4. **Build and run** the project (`F5` or __Debug > Start Debugging__).

### Usage

- Select a puzzle from the dropdown.
- Click **Step** to see the explanation of the rule applied to solve the next number in the puzzle.  
- Use **Clear** to reset the numbers entered, or **New Puzzle** to start fresh.

## Project Structure

- `ViewModels/`: MVVM view models, including `SudokuViewModel`.
- `Models/`: Core data models like `SudokuSquare` and `SolveStep`.
- `Logic/Helpers/`: Helper classes for Sudoku rules and grid management.
- `Logic/Rule/`: Individual solving techniques.
- `Logic/RulesEngine.cs`: Main engine for step-by-step solving.
- `MainWindow.xaml`: WPF UI definition.

## Architecture

Uses the MVVM (Model-View-ViewModel) achitecture pattern for a clean separation of UI and logic.
MVVM (Model–View–ViewModel) is a UI architecture pattern where:

- Model holds the data and business logic,

- View is the UI (XAML), and

- ViewModel acts as the bridge between them.

The Model, View, and ViewModel communicate through data bindings and commands.

- Bindings connect the View to ViewModel properties so UI updates automatically when data changes.
- Commands expose actions from the ViewModel that the View can trigger (e.g., button clicks).
- Notifications (INotifyPropertyChanged) let the View know when a property changes so the UI refreshes dynamically without manual updates.


## License

This project is licensed under the MIT License.

## Credits

Developed by [Ian Jackson](https://github.com/ianjacksonnz).