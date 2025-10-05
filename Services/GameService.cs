using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WordGuessGame.Data;
using WordGuessGame.Models;

namespace WordGuessGame.Services
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public GameService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GameState> StartNewGameAsync(int categoryId)
        {
            var words = await _context.Words
                .Where(w => w.CategoryId == categoryId)
                .Select(w => w.Text)
                .ToListAsync();

            if (!words.Any())
            {
                return null; // Or handle error
            }

            var random = new Random();
            var firstWord = words[random.Next(words.Count)];

            var gameState = new GameState
            {
                CategoryId = categoryId,
                CurrentWord = firstWord,
                Score = 0,
                TimeLeft = 60, // 60 seconds
                UsedWords = new List<string> { firstWord }
            };

            SetGameState(gameState);
            return gameState;
        }

        public async Task<(bool IsCorrect, GameState NewState)> MakeGuessAsync(string guess)
        {
            var gameState = GetGameState();
            if (gameState == null || gameState.IsGameOver)
            {
                return (false, gameState);
            }

            bool isCorrect = string.Equals(gameState.CurrentWord, guess, StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                gameState.Score += 10; // Add 10 points for correct guess

                // Find a new word that hasn't been used yet
                var availableWords = await _context.Words
                    .Where(w => w.CategoryId == gameState.CategoryId && !gameState.UsedWords.Contains(w.Text))
                    .Select(w => w.Text)
                    .ToListAsync();
                
                string newWord = null;
                if (availableWords.Any())
                {
                    var random = new Random();
                    newWord = availableWords[random.Next(availableWords.Count)];
                }
                
                if (newWord != null)
                {
                    gameState.CurrentWord = newWord;
                    gameState.UsedWords.Add(newWord);
                }
                else
                {
                    // No more words in this category, end game
                    gameState.IsGameOver = true;
                }
            }

            SetGameState(gameState);
            return (isCorrect, gameState);
        }

        public GameState GetGameState()
        {
            var json = _session.GetString("GameState");
            return json == null ? null : JsonSerializer.Deserialize<GameState>(json);
        }

        public void SetGameState(GameState gameState)
        {
            var json = JsonSerializer.Serialize(gameState);
            _session.SetString("GameState", json);
        }

        public void ClearGameState()
        {
            _session.Remove("GameState");
        }

        public async Task UpdateTimeLeftAsync(int timeLeft)
        {
            var gameState = GetGameState();
            if (gameState != null)
            {
                gameState.TimeLeft = timeLeft;
                SetGameState(gameState);
            }
        }

        public async Task<(bool wasSaved, int? previousBest)> SaveScoreAsync()
        {
            var gameState = GetGameState();
            if (gameState == null) return (false, null);

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return (false, null);

            // ตรวจสอบคะแนนสูงสุดของผู้เล่นในหมวดหมู่นี้
            var existingBestScore = await _context.Scores
                .Where(s => s.UserId == userId && s.CategoryId == gameState.CategoryId)
                .OrderByDescending(s => s.Points)
                .FirstOrDefaultAsync();

            // บันทึกเฉพาะเมื่อคะแนนใหม่สูงกว่าคะแนนเก่า หรือยังไม่เคยเล่นหมวดหมู่นี้
            if (existingBestScore == null || gameState.Score > existingBestScore.Points)
            {
                var score = new Score
                {
                    Points = gameState.Score,
                    PlayedAt = DateTime.Now,
                    UserId = userId,
                    CategoryId = gameState.CategoryId
                };

                _context.Scores.Add(score);
                await _context.SaveChangesAsync();
                ClearGameState();
                return (true, existingBestScore?.Points);
            }

            ClearGameState();
            return (false, existingBestScore?.Points);
        }
    }
}
