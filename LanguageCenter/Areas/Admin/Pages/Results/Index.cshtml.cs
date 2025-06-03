using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using LanguageCenter.Data;

namespace LanguageCenter.Areas.Admin.Pages.Results
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Result> Results { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public int TotalPages { get; set; }

        private const int PageSize = 10;

        public async Task OnGetAsync()
        {
            var query = _context.Results
                .Include(r => r.Student)
                .Include(r => r.Course)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(r =>
                    r.Student.FullName.Contains(SearchTerm) ||
                    r.Course.Name.Contains(SearchTerm));
            }

            int totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            Results = await query
                .OrderByDescending(r => r.Id)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
