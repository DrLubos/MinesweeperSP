using Minesweeper.Library;
using Microsoft.Maui.Controls.Shapes;

namespace Minesweeper.MAUIApp
{
    public partial class MainPage : ContentPage
    {
        private GameInstance _gameInstance;
        private bool _playingEnabled;
        private int _clickCounter;
        private Timer? _timer;
        private TimeSpan _timeSpan;
        private bool _isTimerRunning;
        private readonly HashSet<string> _visitedCells = [];
        public MainPage()
        {
            InitializeComponent();
            Medium_Click(null, null);
            _gameInstance = new GameInstance(Difficulty.GetRows(GameDifficulty.Medium), Difficulty.GetCols(GameDifficulty.Medium), Difficulty.GetMines(GameDifficulty.Medium));
            _playingEnabled = true;
            _clickCounter = 0;
        }

        private void Easy_Click(object sender, EventArgs e)
        {
            LoadGame(Difficulty.GetRows(GameDifficulty.Easy), Difficulty.GetCols(GameDifficulty.Easy), Difficulty.GetMines(GameDifficulty.Easy));
        }

        private void Medium_Click(object? sender, EventArgs? e)
        {
            LoadGame(Difficulty.GetRows(GameDifficulty.Medium), Difficulty.GetCols(GameDifficulty.Medium), Difficulty.GetMines(GameDifficulty.Medium));
        }

        private void Hard_Click(object sender, EventArgs e)
        {
            LoadGame(Difficulty.GetRows(GameDifficulty.Hard), Difficulty.GetCols(GameDifficulty.Hard), Difficulty.GetMines(GameDifficulty.Hard));
        }

        private void LoadGame(int rows, int cols, int mines)
        {
            MinesCounter.Text = mines.ToString();
            MarkedMinesCounter.Text = "0";
            InitializeGameGrid(rows, cols);
            _gameInstance = new GameInstance(rows, cols, mines);
            _playingEnabled = true;
            _clickCounter = 0;
            TryAgain.IsEnabled = false;
            _timeSpan = TimeSpan.Zero;
            TimerTextBlock.Text = "Time: 00:00:00";
        }

        private void InitializeGameGrid(int rows, int cols)
        {
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < rows; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < cols; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var button = new Button
                    {
                        BackgroundColor = Colors.DarkGreen,
                        BorderColor = Colors.Black,
                        BorderWidth = 1,
                        CornerRadius = 0,
                        ClassId = $"{i},{j}",
                        Text = "",
                        TextColor = Colors.Black,
                        FontSize = 20,
                    };
                    button.Released += OnRightClick;
                    button.Clicked += Button_Clicked;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    GameGrid.Children.Add(button);
                }
            }
        }

        private void OnRightClick(object? sender, EventArgs? e)
        {
            if (!_playingEnabled)
            {
                return;
            }
            if (sender is not Button button)
            {
                return;
            }
            if (button.Text == "M")
            {
                button.Text = "";
                button.BackgroundColor = Colors.DarkGreen;
                MinesCounter.Text = (int.Parse(MinesCounter.Text) + 1).ToString();
                MarkedMinesCounter.Text = (int.Parse(MarkedMinesCounter.Text) - 1).ToString();
            }
            else
            {
                if (int.Parse(MinesCounter.Text) == 0)
                {
                    return;
                }
                button.Text = "M";
                button.BackgroundColor = Colors.DarkRed;
                MinesCounter.Text = (int.Parse(MinesCounter.Text) - 1).ToString();
                MarkedMinesCounter.Text = (int.Parse(MarkedMinesCounter.Text) + 1).ToString();
            }
        }

        private void Button_Clicked(object? sender, EventArgs? e)
        {
            _visitedCells.Clear();
            if (!_playingEnabled)
            {
                return;
            }
            if (sender is not Button)
            {
                return;
            }
            if (!_isTimerRunning)
            {
                _isTimerRunning = true;
                InitializeTimer();
            }
            var button = (Button)sender;
            var coords = button.ClassId.Split(',');
            var row = int.Parse(coords[0]);
            var col = int.Parse(coords[1]);
            var result = _gameInstance.SelectedTile(row, col);
            if (result == null)
            {
                return;
            }
            if (result.Item1)
            {
                RevealMines();
                GameFinished(false);
            }
            else
            {
                RevealTile(row, col);
                if (_clickCounter == _gameInstance.Rows * _gameInstance.Cols - _gameInstance.Mines)
                {
                    GameFinished(true);
                }
            }
        }

        private void RevealMines()
        {
            foreach (var b in GameGrid.Children)
            {
                if (b is not Button btn)
                {
                    continue;
                }
                var position = btn.ClassId.Split(',');
                var rowFor = int.Parse(position[0]);
                var colFor = int.Parse(position[1]);
                var r = _gameInstance.SelectedTile(rowFor, colFor);
                if (r == null)
                {
                    continue;
                }
                if (btn.Text == "M" && r.Item1 == false)
                {
                    btn.BackgroundColor = Colors.Yellow;
                }
                if (btn.Text == "M" && r.Item1)
                {
                    continue;
                }
                if (r.Item1)
                {
                    btn.Text = "X";
                    btn.BackgroundColor = Colors.Red;
                }
            }
        }
        private void RevealTile(int row, int col)
        {
            string cellId = $"{row},{col}";
            if (_visitedCells.Contains(cellId))
            {
                return;
            }
            var result = _gameInstance.SelectedTile(row, col);
            if (result == null || result.Item1)
            {
                return;
            }
            var button = GetButtonAt(row, col);
            if (button == null || !button.IsEnabled)
            {
                return;
            }
            _visitedCells.Add(cellId);
            if (button.Text == "M")
            {
                MinesCounter.Text = (int.Parse(MinesCounter.Text) + 1).ToString();
                MarkedMinesCounter.Text = (int.Parse(MarkedMinesCounter.Text) - 1).ToString();
            }
            RemoveButton(button);
            Border border = new()
            {
                Stroke = Colors.Black,
                Margin = -1,
                StrokeThickness = 2,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(0, 0, 0, 0)
                },
                Content = new Label
                {
                    Text = result.Item2 == 0 ? "" : result.Item2.ToString(),
                    TextColor = MainPage.GetColor(result.Item2),
                    FontSize = 25,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            GameGrid.Children.Add(border);
            ++_clickCounter;
            if (result.Item2 == 0)
            {
                for (int rowChange = -1; rowChange <= 1; ++rowChange)
                {
                    for (int colChange = -1; colChange <= 1; ++colChange)
                    {
                        int r = row + rowChange;
                        int c = col + colChange;
                        if (r >= 0 && r < _gameInstance.Rows && c >= 0 && c < _gameInstance.Cols)
                        {
                            RevealTile(r, c);
                        }
                    }
                }
            }
        }

        private void RemoveButton(Button button)
        {
            button.Clicked -= Button_Clicked;
            if (GameGrid.Contains(button))
            {
                GameGrid.Children.Remove(button);
            }
        }
        private Button? GetButtonAt(int row, int col)
        {
            foreach (var b in GameGrid.Children)
            {
                if (b is not Button button)
                {
                    continue;
                }
                var position = button.ClassId.Split(',');
                var rowButton = int.Parse(position[0]);
                var colButton = int.Parse(position[1]);
                if (rowButton == row && colButton == col)
                {
                    return button;
                }
            }
            return null;
        }

        private static Color GetColor(int number)
        {
            return number switch
            {
                1 => Colors.Blue,
                2 => Colors.Green,
                3 => Colors.Red,
                4 => Colors.Purple,
                5 => Colors.Maroon,
                6 => Colors.Turquoise,
                7 => Colors.Sienna,
                8 => Colors.OrangeRed,
                9 => Colors.Pink,
                _ => Colors.Black
            };
        }

        private void TryAgain_Click(object sender, EventArgs e)
        {
            LoadGame(_gameInstance.Rows, _gameInstance.Cols, _gameInstance.Mines);
        }

        private void GameFinished(bool win)
        {
            _timer?.Dispose();
            _isTimerRunning = false;
            _playingEnabled = false;
            TryAgain.IsEnabled = true;
            if (win)
            {
                DisplayAlert("You won", "You won!", "OK");
            }
            else
            {
                DisplayAlert("You lost", "You lost!", "OK");
            }
        }

        private void InitializeTimer()
        {
            _timeSpan = TimeSpan.Zero;
            _timer = new Timer(UpdateTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateTimer(object? state)
        {
            _timeSpan = _timeSpan.Add(TimeSpan.FromMilliseconds(1));
            // MainThread generated by AI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerTextBlock.Text = "Time: " + _timeSpan.ToString(@"hh\:mm\:ss");
            });
        }

        private void About_Click(object sender, EventArgs e)
        {
            DisplayAlert("About", "Minesweeeper Game\nVersion 1.0\nCopyright (c) 2024 Ľuboš Dragan", "OK");
        }
        private void HowToPlay_Click(object sender, EventArgs e)
        {
            DisplayAlert("How to play Minesweeper", "Left click to reveal a tile.\nRight click to mark a mine.\nTry to reveal all tiles without revealing a mine.", "OK");
        }
    }

}
