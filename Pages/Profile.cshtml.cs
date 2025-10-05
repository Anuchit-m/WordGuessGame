using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WordGuessGame.Data;
using WordGuessGame.Models;

namespace WordGuessGame.Pages
{
    [Authorize] // Only logged-in users can see this page
    public class ProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public string Email { get; set; }
        public int HighestScore { get; set; }
        public int GamesPlayed { get; set; }
        public double AverageScore { get; set; }
        public List<Score> RecentScores { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userScores = await _context.Scores
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.PlayedAt)
                .ToListAsync();

            Email = user.Email;
            GamesPlayed = userScores.Count;

            if (GamesPlayed > 0)
            {
                HighestScore = userScores.Max(s => s.Points);
                AverageScore = userScores.Average(s => s.Points);
                RecentScores = userScores.Take(10).ToList();
            }
            else
            {
                HighestScore = 0;
                AverageScore = 0;
                RecentScores = new List<Score>();
            }
            
            return Page();
        }
    }
}
