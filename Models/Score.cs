using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordGuessGame.Models
{
    public class Score
    {
        public int Id { get; set; }

        [Required]
        public int Points { get; set; }

        public DateTime PlayedAt { get; set; }

        // Foreign Key to link to the user (from ASP.NET Core Identity)
        [Required]
        public string UserId { get; set; }

        // Foreign Key to link to the category
        [Required]
        public int CategoryId { get; set; }

        // Navigation property: A Score belongs to one User
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        // Navigation property: A Score belongs to one Category
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
