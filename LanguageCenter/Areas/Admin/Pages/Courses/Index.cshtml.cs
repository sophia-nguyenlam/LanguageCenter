using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Course> Courses { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? LevelFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(SearchTerm));
            }

            if (!string.IsNullOrEmpty(LevelFilter))
            {
                query = query.Where(c => c.Level == LevelFilter);
            }

            int totalCourses = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCourses / (double)PageSize);

            Courses = await query
                .OrderBy(c => c.Name)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}
