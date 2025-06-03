using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.ClassSessions
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ClassSession> ClassSessions { get; set; } = new();
        public string? SearchTerm { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string? searchTerm, int page = 1)
        {
            SearchTerm = searchTerm;
            Page = page;

            var query = _context.ClassSessions
                .Include(cs => cs.Course)
                .Include(cs => cs.Teacher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(cs =>
                    cs.Course.Name.Contains(SearchTerm) ||
                    cs.Teacher.FullName.Contains(SearchTerm));
            }

            int totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            ClassSessions = await query
                .OrderByDescending(cs => cs.StartTime)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
