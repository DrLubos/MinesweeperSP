using System.Windows;

namespace Minesweeper.WPFApp
{
    /// <summary>
    /// Interaction logic for CustomDifficultySelect.xaml
    /// </summary>
    public partial class CustomDifficultySelect
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public int Mines { get; private set; }

        public CustomDifficultySelect()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsTextBox.Text, out int rows) &&
                int.TryParse(ColsTextBox.Text, out int cols) &&
                int.TryParse(MinesTextBox.Text, out int mines))
            {
                Rows = rows;
                Cols = cols;
                Mines = mines;
                if (rows < 1 || cols < 1 || mines < 1)
                {
                    MessageBox.Show("All values must be greater than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (mines >= rows * cols)
                {
                    MessageBox.Show("Number of mines must be less than the number of tiles", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Invalid input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
