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
                GameDifficulty.Easy => 8,
                GameDifficulty.Medium => 14,
                GameDifficulty.Hard => 24,
                _ => 16,
            };
        }

        public static int GetCols(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Easy => 10,
                GameDifficulty.Medium => 18,
                GameDifficulty.Hard => 20,
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
