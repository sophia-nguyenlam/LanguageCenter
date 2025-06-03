using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LanguageCenter.Data;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Areas.Admin.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var lowerSearchTerm = SearchTerm.ToLower();
                query = query.Where(u =>
                    (u.FullName ?? "").ToLower().Contains(lowerSearchTerm) ||
                    (u.Email ?? "").ToLower().Contains(lowerSearchTerm));
            }

            if (StatusFilter == "active")
            {
                query = query.Where(u => u.IsActive);
            }
            else if (StatusFilter == "inactive")
            {
                query = query.Where(u => !u.IsActive);
            }

            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            if (Page < 1) Page = 1;
            if (TotalPages > 0 && Page > TotalPages) Page = TotalPages;

            Users = await query
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedDate)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
