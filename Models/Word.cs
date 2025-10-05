using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordGuessGame.Models
{
    public class Word
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Text { get; set; }

        // Foreign Key to link to the Category
        public int CategoryId { get; set; }

        // Navigation property: A Word belongs to one Category
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
