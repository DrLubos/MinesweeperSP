namespace Minesweeper.MAUIApp
{
    public class CustomDifficultyEventArgs(int rows, int cols, int mines) : EventArgs
    {
        public int Rows { get; } = rows;
        public int Cols { get; } = cols;
        public int Mines { get; } = mines;
    }
}
