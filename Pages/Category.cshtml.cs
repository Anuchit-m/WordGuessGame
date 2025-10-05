using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WordGuessGame.Data;
using WordGuessGame.Models;

namespace WordGuessGame.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CategoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Categories { get; set; }

        public async Task OnGetAsync()
        {
            // ดึงข้อมูลหมวดหมู่ทั้งหมดจากฐานข้อมูล
            Categories = await _context.Categories.ToListAsync();
        }
    }
}
