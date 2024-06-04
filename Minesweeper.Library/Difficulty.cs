namespace Minesweeper.Library
{
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard,
    }

    public static class Difficulty
    {
        public static int GetRows(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Easy => 9,
                GameDifficulty.Medium => 16,
                GameDifficulty.Hard => 16,
                _ => 16,
            };
        }

        public static int GetCols(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Easy => 9,
                GameDifficulty.Medium => 16,
                GameDifficulty.Hard => 30,
                _ => 16,
            };
        }

        public static int GetMines(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Easy => 10,
                GameDifficulty.Medium => 40,
                GameDifficulty.Hard => 99,
                _ => 40,
            };
        }
    }
}
