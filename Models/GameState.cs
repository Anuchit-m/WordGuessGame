namespace WordGuessGame.Models
{
    public class GameState
    {
        public string CurrentWord { get; set; }
        public int Score { get; set; }
        public int TimeLeft { get; set; }
        public int CategoryId { get; set; }
        public List<string> UsedWords { get; set; } = new List<string>();
        public bool IsGameOver { get; set; } = false;
    }
}
