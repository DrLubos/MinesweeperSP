using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Minesweeper.Library;

namespace Minesweeper.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private GameInstance _minesweeperGame;
        private bool _playingEnabled;
        private int _clickCounter;
        private readonly DispatcherTimer _dispatcherTimer = new();
        private readonly Stopwatch _stopwatch = new();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            Medium_Click(null, null);
            _minesweeperGame = new GameInstance(Difficulty.GetRows(GameDifficulty.Medium), Difficulty.GetCols(GameDifficulty.Medium), Difficulty.GetMines(GameDifficulty.Medium));
            _playingEnabled = true;
            _clickCounter = 0;
        }

        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            LoadGame(Difficulty.GetRows(GameDifficulty.Easy), Difficulty.GetCols(GameDifficulty.Easy), Difficulty.GetMines(GameDifficulty.Easy));
        }
        private void Medium_Click(object? sender, RoutedEventArgs? e)
        {
            LoadGame(Difficulty.GetRows(GameDifficulty.Medium), Difficulty.GetCols(GameDifficulty.Medium), Difficulty.GetMines(GameDifficulty.Medium));
        }
        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            LoadGame(Difficulty.GetRows(GameDifficulty.Hard), Difficulty.GetCols(GameDifficulty.Hard), Difficulty.GetMines(GameDifficulty.Hard));
        }

        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            var customDifficultySelect = new CustomDifficultySelect();
            if (customDifficultySelect.ShowDialog() == true)
            {
                LoadGame(customDifficultySelect.Rows, customDifficultySelect.Cols, customDifficultySelect.Mines);
            }
        }

        private void LoadGame(int rows, int cols, int mines)
        {
            MinesCounter.Text = mines.ToString();
            MarkedMinesCounter.Text = "0";
            InitializeGameGrid(rows, cols);
            _minesweeperGame = new GameInstance(rows, cols, mines);
            _playingEnabled = true;
            _clickCounter = 0;
            TryAgain.IsEnabled = false;
            TimerTextBlock.Text = "Time: 00:00:00";
            _stopwatch.Reset();
        }

        private void InitializeGameGrid(int rows, int cols)
        {
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < rows; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    Button button = new()
                    {
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Tag = new Tuple<int, int>(row, col)
                    };
                    button.Click += Button_Click;
                    button.MouseRightButtonUp += Button_MouseRightButtonUp;
                    button.IsEnabled = true;
                    button.Background = Brushes.DarkGreen;
                    button.FontSize = 20;
                    button.FontWeight = FontWeights.Bold;
                    button.Content = "";
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    GameGrid.Children.Add(button);
                }
            }
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_playingEnabled)
            {
                return;
            }
            Button button = (Button)sender;
            if (button.Content.ToString() == "M")
            {
                button.Content = "";
                MinesCounter.Text = (int.Parse(MinesCounter.Text) + 1).ToString();
                MarkedMinesCounter.Text = (int.Parse(MarkedMinesCounter.Text) - 1).ToString();
                button.Background = Brushes.DarkGreen;
                return;
            }
            if (MinesCounter.Text == "0")
            {
                return;
            }
            button.Content = "M";
            button.Background = Brushes.DarkRed;
            MinesCounter.Text = (int.Parse(MinesCounter.Text) - 1).ToString();
            MarkedMinesCounter.Text = (int.Parse(MarkedMinesCounter.Text) + 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_playingEnabled)
            {
                return;
            }
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
                _dispatcherTimer.Start();
            }
            Button button = (Button)sender;
            if (button.Content.ToString() == "M")
            {
                return;
            }
            Tuple<int, int> position = (Tuple<int, int>)button.Tag;
            var result = _minesweeperGame.SelectedTile(position.Item1, position.Item2);
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
                RevealTile(position.Item1, position.Item2);
                if (_clickCounter == _minesweeperGame.Rows * _minesweeperGame.Cols - _minesweeperGame.Mines)
                {
                    GameFinished(true);
                }
            }
        }

        private void RevealMines()
        {
            foreach (Button b in GameGrid.Children)
            {
                Tuple<int, int> pos = (Tuple<int, int>)b.Tag;
                var r = _minesweeperGame.SelectedTile(pos.Item1, pos.Item2);
                if (r == null)
                {
                    continue;
                }
                if (b.Content.ToString() == "M" && r.Item1 == false)
                {
                    b.Background = Brushes.Yellow;
                }

                if (b.Content.ToString() == "M" && r.Item1)
                {
                    continue;
                }
                if (r.Item1)
                {
                    b.Content = "X";
                    b.Background = Brushes.Red;
                }
            }
        }

        private void RevealTile(int row, int col)
        {
            var result = _minesweeperGame.SelectedTile(row, col);
            if (result == null || result.Item1)
            {
                return;
            }
            var button = GetButtonAt(row, col);
            if (button == null || !button.IsEnabled)
            {
                return;
            }
            button.Content = result.Item2 == 0 ? "" : result.Item2.ToString();
            button.Background = Brushes.LightGray;
            button.Foreground = GetColor(result.Item2);
            button.IsEnabled = false;
            ++_clickCounter;
            if (result.Item2 == 0)
            {
                for (int rowChange = -1; rowChange <= 1; ++rowChange)
                {
                    for (int colChange = -1; colChange <= 1; ++colChange)
                    {
                        int r = row + rowChange;
                        int c = col + colChange;
                        if (r >= 0 && r < _minesweeperGame.Rows && c >= 0 && c < _minesweeperGame.Cols)
                        {
                            RevealTile(r, c);
                        }
                    }
                }
            }
        }

        private Button? GetButtonAt(int row, int col)
        {
            foreach (Button button in GameGrid.Children)
            {
                Tuple<int, int> position = (Tuple<int, int>)button.Tag;
                if (position.Item1 == row && position.Item2 == col)
                {
                    return button;
                }
            }
            return null;
        }

        private static SolidColorBrush GetColor(int number)
        {
            return number switch
            {
                1 => Brushes.Blue,
                2 => Brushes.Green,
                3 => Brushes.Red,
                4 => Brushes.Purple,
                5 => Brushes.Maroon,
                6 => Brushes.Turquoise,
                7 => Brushes.Sienna,
                8 => Brushes.OrangeRed,
                9 => Brushes.Pink,
                _ => Brushes.Black
            };
        }

        private void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            LoadGame(_minesweeperGame.Rows, _minesweeperGame.Cols, _minesweeperGame.Mines);
        }

        private void GameFinished(bool win)
        {
            _stopwatch.Stop();
            _dispatcherTimer.Stop();
            _playingEnabled = false;
            TryAgain.IsEnabled = true;
            if (win)
            {
                MessageBox.Show(this, "You Won", "You Won", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "Game Over", "Game Over", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void InitializeTimer()
        {
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            TimerTextBlock.Text = "Time: " + _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new Window
            {
                Title = "About",
                Width = 270,
                Height = 170,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false
            };
            var stackPanel = new StackPanel();
            var title = new TextBlock { Text = "Minesweeper Game", Margin = new Thickness(10, 10, 0, 10), FontWeight = FontWeights.Bold, FontSize = 20 };
            var version = new TextBlock { Text = "Version 1.0", Margin = new Thickness(10, 0, 0, 0), FontSize = 16 };
            var copyright = new TextBlock { Text = "Copyright (c) 2024 Ľuboš Dragan", Margin = new Thickness(10, 0, 0, 0), FontSize = 16 };
            var button = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(10), FontSize = 16 };
            stackPanel.Children.Add(title);
            stackPanel.Children.Add(version);
            stackPanel.Children.Add(copyright);
            stackPanel.Children.Add(button);
            aboutWindow.Content = stackPanel;
            button.Click += (_, _) =>
            {
                aboutWindow.Close();
            };
            aboutWindow.ShowDialog();
        }

        private void HowToPlay_Click(object sender, RoutedEventArgs e)
        {
            var howToPlayWindow = new Window
            {
                Title = "How to play",
                Width = 370,
                Height = 235,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false
            };
            var stackPanel = new StackPanel();
            var title = new TextBlock { Text = "How to play Minesweeper", Margin = new Thickness(10, 10, 0, 10), FontWeight = FontWeights.Bold, FontSize = 20 };
            var text = new TextBlock { Text = "Left click to reveal a tile.\nRight click to mark a mine.\nNumber in tile represents how many mines\nis around the tile.\nTry to reveal all tiles without revealing a mine.", Margin = new Thickness(10, 0, 0, 0), FontSize = 16 };
            var button = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(10), FontSize = 16 };
            stackPanel.Children.Add(title);
            stackPanel.Children.Add(text);
            stackPanel.Children.Add(button);
            howToPlayWindow.Content = stackPanel;
            button.Click += (_, _) =>
            {
                howToPlayWindow.Close();
            };
            howToPlayWindow.ShowDialog();
        }
    }
}