﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Minesweeper.Library;

namespace Minesweeper.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameInstance _minesweeperGame;

        public MainWindow()
        {
            InitializeComponent();
            Medium_Click(null, null);
            _minesweeperGame = new GameInstance(Difficulty.GetRows(GameDifficulty.Medium), Difficulty.GetCols(GameDifficulty.Medium), Difficulty.GetMines(GameDifficulty.Medium));
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
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
            InitializeGameGrid(rows, cols);
            _minesweeperGame = new GameInstance(rows, cols, mines);
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
                    button.Content = "";
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    GameGrid.Children.Add(button);
                }
            }
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Content.ToString() == "M")
            {
                button.Content = "";
                MinesCounter.Text = (int.Parse(MinesCounter.Text) + 1).ToString();
                return;
            }
            button.Content = "M";
            button.Background = Brushes.Red;
            MinesCounter.Text = (int.Parse(MinesCounter.Text) - 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
                MessageBox.Show("Game Over", "Game Over", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                button.Content = result.Item2 == 0 ? "" : result.Item2.ToString();
                button.Background = Brushes.LightGray;
                RevealNeighbours(position.Item1, position.Item2);
            }
        }

        private void RevealNeighbours(int row, int col)
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
            button.IsEnabled = false;
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
                            RevealNeighbours(r, c);
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
    }
}