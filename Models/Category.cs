using System.ComponentModel.DataAnnotations;

namespace WordGuessGame.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // Navigation property: One Category can have many Words
        public ICollection<Word> Words { get; set; } = new List<Word>();
    }
}
