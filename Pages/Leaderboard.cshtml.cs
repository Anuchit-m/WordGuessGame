using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WordGuessGame.Data;
using WordGuessGame.Models;

namespace WordGuessGame.Pages
{
    public class LeaderboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ScoreViewModel> Scores { get; set; }

        public async Task OnGetAsync()
        {
            Scores = await _context.Scores
                .Include(s => s.User) // Include User data to get the email/username
                .Include(s => s.Category) // Include Category data to get the category name
                .OrderByDescending(s => s.Points)
                .Take(20) // Get top 20 scores
                .Select(s => new ScoreViewModel
                {
                    UserName = s.User.UserName,
                    Points = s.Points,
                    PlayedAt = s.PlayedAt,
                    CategoryName = s.Category.Name
                })
                .ToListAsync();
        }

        public class ScoreViewModel
        {
            public string UserName { get; set; }
            public int Points { get; set; }
            public DateTime PlayedAt { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
