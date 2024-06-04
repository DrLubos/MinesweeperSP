namespace Minesweeper.Library
{
    public class GameInstance
    {
        private readonly int[,] _board;
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _mines;
        private readonly Random _random = new();

        public GameInstance(int rows, int cols, int mines)
        {
            _rows = rows;
            _cols = cols;
            _mines = mines;
            _board = new int[rows, cols];
            PlaceMines();
        }

        private void PlaceMines()
        {
            for (int i = 0; i < _mines; ++i)
            {
                int row, col;
                do
                {
                    row = _random.Next(_rows);
                    col = _random.Next(_cols);
                } while (_board[row, col] == -1);
                _board[row, col] = -1;
                for (int rowChange = -1; rowChange <= 1; ++rowChange)
                {
                    for (int colChange = -1; colChange <= 1; ++colChange)
                    {
                        int r = row + rowChange;
                        int c = col + colChange;
                        if (r >= 0 && r < _rows && c >= 0 && c < _cols && _board[r, c] != -1)
                        {
                            ++_board[r, c];
                        }
                    }
                }
            }
        }

        public Tuple<bool, int>? SelectedTile(int row, int col)
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
            {
                return null;
            }
            Tuple<bool, int> result = _board[row, col] == -1 ? new Tuple<bool, int>(true, -1) : new Tuple<bool, int>(false, _board[row, col]);
            return result;
        }

        public int Rows => _rows;
        public int Cols => _cols;
        public int Mines => _mines;
    }
}
