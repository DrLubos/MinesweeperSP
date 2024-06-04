using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minesweeper.WPFApp
{
    /// <summary>
    /// Interaction logic for CustomDifficultySelect.xaml
    /// </summary>
    public partial class CustomDifficultySelect : Window
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
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Invalid input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
