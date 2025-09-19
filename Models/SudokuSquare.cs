using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace SudokuStepByStep.Models
{
    public class SudokuSquare : INotifyPropertyChanged
    {
        private int _number;
        private bool _isReadOnly;
        private Brush _backgroundColor = Brushes.White;
        private ObservableCollection<int> _possibleNumbers = new ObservableCollection<int>();

        public int Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                    OnPropertyChanged(nameof(DisplayNumber)); // Notify when DisplayNumber changes
                }
            }
        }

        // Add this property to hide zeros in the UI
        public string DisplayNumber => _number == 0 ? string.Empty : _number.ToString();

        public ObservableCollection<int> PossibleNumbers
        {
            get => _possibleNumbers;
            set
            {
                if (_possibleNumbers != value)
                {
                    _possibleNumbers = value;
                    OnPropertyChanged(nameof(PossibleNumbers));
                }
            }
        }

        public int Row { get; set; }
        public int Column { get; set; }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    OnPropertyChanged(nameof(BackgroundColor));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
